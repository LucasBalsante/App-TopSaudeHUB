import { SelectionModel } from '@angular/cdk/collections';
import { ComponentType } from '@angular/cdk/portal';
import {
    AfterViewInit,
    ChangeDetectionStrategy,
    Component,
    ViewChild,
    effect,
    inject,
    input,
    output,
    signal
} from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { Dialog, DialogData } from '../dialog/dialog';

export type DataTableCellValue = string | number | boolean | Date | null | undefined;

export type DataTableRow = Record<string, DataTableCellValue>;

export interface DataTableColumn {
    key: string;
    label: string;
    sortable?: boolean;
    sticky?: boolean;
    stickyEnd?: boolean;
    textAlign?: 'start' | 'center' | 'end';
    footer?: string | number | ((rows: DataTableRow[]) => string | number);
    cell?: (row: DataTableRow) => DataTableCellValue;
}

export interface DataTableRowAction {
    key: string;
    label: string;
}

export type DataTableDialogDataBuilder = (row: DataTableRow | undefined, title: string) => unknown;

export interface DataTableDialogSubmittedEvent {
    mode: 'create' | 'edit';
    row?: DataTableRow;
    value: unknown;
}

const SELECT_COLUMN_KEY = '__select';
const ACTIONS_COLUMN_KEY = '__actions';

/**
 * @title Data table with sorting, pagination, filtering, selection, footer and sticky support.
 */
@Component({
    selector: 'data-table',
    standalone: true,
    styleUrl: 'data-table.css',
    templateUrl: 'data-table.html',
    imports: [
        MatButtonModule,
        MatCheckboxModule,
        MatDialogModule,
        MatFormFieldModule,
        MatIconModule,
        MatInputModule,
        MatMenuModule,
        MatTableModule,
        MatSortModule,
        MatPaginatorModule
    ],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class DataTable implements AfterViewInit {
    readonly columns = input.required<DataTableColumn[]>();
    readonly data = input.required<DataTableRow[]>();
    readonly filterPlaceholder = input('Filtrar registros');
    readonly noDataMessage = input('Nenhum dado correspondente ao filtro atual.');
    readonly ariaLabel = input('Tabela de dados');
    readonly role = input<'table' | 'grid' | 'treegrid'>('table');
    readonly pageSizeOptions = input<number[]>([5, 10, 25, 100]);
    readonly initialPageSize = input(5);
    readonly enableSelection = input(false);
    readonly stickyHeader = input(false);
    readonly stickyFooter = input(false);
    readonly enableRowClickLog = input(false);
    readonly enableRowActions = input(false);
    readonly rowActions = input<DataTableRowAction[]>([
        { key: 'edit', label: 'Editar' },
        { key: 'cancel', label: 'Cancelar' }
    ]);
    readonly dialogComponent = input<ComponentType<unknown> | null>(null);
    readonly dialogDataBuilder = input<DataTableDialogDataBuilder | null>(null);
    readonly dialogWidth = input('480px');
    readonly newDialogTitle = input('Novo cadastro');
    readonly editDialogTitle = input('Editar cadastro');
    readonly dialog = inject(MatDialog);

    readonly dialogSubmitted = output<DataTableDialogSubmittedEvent>();
    readonly rowClicked = output<DataTableRow>();
    readonly rowActionClicked = output<{ action: DataTableRowAction; row: DataTableRow }>();
    readonly selectionChanged = output<DataTableRow[]>();
    readonly selection = new SelectionModel<DataTableRow>(true, []);
    readonly dataSource = new MatTableDataSource<DataTableRow>([]);
    readonly clickedRowLog = signal('Nenhuma linha selecionada.');

    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;

    constructor() {
        effect(() => {
            const rows = this.data();

            this.dataSource.data = rows;
            this.selection.clear();
            this.selectionChanged.emit([]);

            if (this.paginator) {
                this.paginator.firstPage();
            }
        });

        this.dataSource.filterPredicate = (row, filter) => {
            const normalizedFilter = filter.trim().toLowerCase();
            const rowContent = this.columns()
                .map((column) => this.getCellValue(row, column))
                .join(' ')
                .toLowerCase();

            return rowContent.includes(normalizedFilter);
        };
    }

    ngAfterViewInit(): void {
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.paginator.pageSize = this.initialPageSize();
        this.dataSource.sortingDataAccessor = (row, columnId) => {
            const column = this.columns().find((item) => item.key === columnId);
            const value = column ? this.resolveCellValue(row, column) : row[columnId];

            if (value instanceof Date) {
                return value.getTime();
            }

            if (typeof value === 'number') {
                return value;
            }

            if (typeof value === 'boolean') {
                return value ? 1 : 0;
            }

            return String(value ?? '').toLowerCase();
        };
    }

    openDialog(row?: DataTableRow, title = this.newDialogTitle()): void {
        const dialogDataBuilder = this.dialogDataBuilder();
        const dialogData = dialogDataBuilder ? dialogDataBuilder(row, title) : this.buildDialogData(row, title);
        const dialogComponent = this.dialogComponent() ?? Dialog;

        const dialogRef = this.dialog.open(dialogComponent, {
            width: this.dialogWidth(),
            data: dialogData,
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result !== undefined) {
                this.dialogSubmitted.emit({
                    mode: row ? 'edit' : 'create',
                    row,
                    value: result
                });

                this.clickedRowLog.set(JSON.stringify(result));
            }
        });
    }

    applyFilter(event: Event): void {
        const filterValue = (event.target as HTMLInputElement).value;
        this.dataSource.filter = filterValue.trim().toLowerCase();

        if (this.dataSource.paginator) {
            this.dataSource.paginator.firstPage();
        }
    }

    isAllSelected(): boolean {
        return this.selection.selected.length === this.dataSource.filteredData.length;
    }

    toggleAllRows(): void {
        if (this.isAllSelected()) {
            this.selection.clear();
            this.selectionChanged.emit([]);
            return;
        }

        this.selection.clear();
        this.dataSource.filteredData.forEach((row) => this.selection.select(row));
        this.selectionChanged.emit([...this.selection.selected]);
    }

    toggleRow(row: DataTableRow): void {
        this.selection.toggle(row);
        this.selectionChanged.emit([...this.selection.selected]);
    }

    handleRowClick(row: DataTableRow): void {
        this.rowClicked.emit(row);

        if (this.enableRowClickLog()) {
            this.clickedRowLog.set(JSON.stringify(row));
        }
    }

    get displayedColumns(): string[] {
        const columnKeys = this.columns().map((column) => column.key);
        const columns = this.enableSelection() ? [SELECT_COLUMN_KEY, ...columnKeys] : columnKeys;

        return this.enableRowActions() ? [...columns, ACTIONS_COLUMN_KEY] : columns;
    }

    get hasFooter(): boolean {
        return this.columns().some((column) => column.footer !== undefined);
    }

    get noDataColspan(): number {
        return this.displayedColumns.length;
    }

    get selectColumnKey(): string {
        return SELECT_COLUMN_KEY;
    }

    get actionsColumnKey(): string {
        return ACTIONS_COLUMN_KEY;
    }

    getCellValue(row: DataTableRow, column: DataTableColumn): string {
        const value = this.resolveCellValue(row, column);

        if (value instanceof Date) {
            return value.toLocaleDateString('pt-BR');
        }

        return String(value ?? '-');
    }

    getFooterValue(column: DataTableColumn): string {
        if (typeof column.footer === 'function') {
            return String(column.footer(this.data()));
        }

        return String(column.footer ?? '');
    }

    trackByColumnKey(_index: number, column: DataTableColumn): string {
        return column.key;
    }

    handleRowAction(action: DataTableRowAction, row: DataTableRow): void {
        if (action.key === 'edit') {
            this.openDialog(row, this.editDialogTitle());
        }

        this.rowActionClicked.emit({ action, row });
    }

    private buildDialogData(row?: DataTableRow, title?: string): DialogData {
        const visibleColumns = this.columns();
        const primaryColumn = visibleColumns[0];
        const secondaryColumn = visibleColumns[1];

        return {
            title: title ?? this.newDialogTitle(),
            confirmLabel: 'Salvar',
            cancelLabel: 'Cancelar',
            name: row && primaryColumn ? this.getCellValue(row, primaryColumn) : '',
            animal: row && secondaryColumn ? this.getCellValue(row, secondaryColumn) : ''
        };
    }

    private resolveCellValue(row: DataTableRow, column: DataTableColumn): DataTableCellValue {
        return column.cell ? column.cell(row) : row[column.key];
    }
}
