import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import {
    DataTable,
    DataTableColumn,
    DataTableDialogDataBuilder,
    DataTableDialogSubmittedEvent,
    DataTableRowAction,
    DataTableRow
} from '../../../../../styles/components/data-table/data-table';
import {
    OrderFormDialogComponent,
    OrderFormDialogData,
    OrderFormValue
} from '../../components/order-form-dialog/order-form-dialog.component';
import { SidenavResponsive } from '../../../../../styles/components/sidebar/sidenav-responsive';
import {
    ApiFeedbackDialogComponent,
    ApiFeedbackDialogData
} from '../../../../../styles/components/dialog/api-feedback-dialog.component';

@Component({
    selector: 'app-home-page',
    standalone: true,
    imports: [SidenavResponsive, DataTable],
    templateUrl: './home-page.component.html',
    styleUrl: './home-page.component.css',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomePageComponent {
    private readonly dialog = inject(MatDialog);

    protected readonly orderDialogComponent = OrderFormDialogComponent;

    protected readonly rowActions: DataTableRowAction[] = [
        { key: 'edit', label: 'Editar' },
        { key: 'pay-order', label: 'Pagar Pedido' },
        { key: 'cancel-order', label: 'Cancelar Pedido' }
    ];

    protected readonly tableColumns: DataTableColumn[] = [
        { key: 'cliente', label: 'Cliente', sortable: true },
        {
            key: 'totalPedido',
            label: 'Total do Pedido',
            sortable: true,
            cell: (row) => this.formatCurrency(Number(row['totalPedido'] ?? 0))
        },
        { key: 'status', label: 'Status', sortable: true },
    ];

    protected readonly orderDialogDataBuilder: DataTableDialogDataBuilder = (row, title) => {
        const dialogData: OrderFormDialogData = {
            title,
            confirmLabel: 'Salvar',
            cancelLabel: 'Cancelar'
        };

        if (row) {
            dialogData.order = {
                cliente: String(row['cliente'] ?? ''),
                totalPedido: Number(row['totalPedido'] ?? 0),
                status: String(row['status'] ?? '')
            };
        }

        return dialogData;
    };

    protected readonly tableData = signal<DataTableRow[]>([
        { id: 1, cliente: 'Mariana Costa', totalPedido: 150, status: 'Ativo' },
        { id: 2, cliente: 'Carlos Lima', totalPedido: 200, status: 'Em analise' },
        { id: 3, cliente: 'Fernanda Alves', totalPedido: 180, status: 'Pendente' },
        { id: 4, cliente: 'Rafael Souza', totalPedido: 220, status: 'Ativo' },
        { id: 5, cliente: 'Rafael Souza', totalPedido: 220, status: 'Ativo' },
        { id: 6, cliente: 'Rafael Souza', totalPedido: 220, status: 'Ativo' },
        { id: 7, cliente: 'Rafael Souza', totalPedido: 220, status: 'Ativo' }
    ]);

    protected handleDialogSubmitted(event: DataTableDialogSubmittedEvent): void {
        const formValue = event.value as OrderFormValue;

        if (event.mode === 'create') {
            this.tableData.update((rows) => [...rows, this.mapOrderToRow(formValue, this.getNextId(rows))]);
            this.openFeedbackDialog({
                title: 'Pedido cadastrado',
                message: 'Pedido cadastrado com sucesso.',
                tone: 'success'
            });
            return;
        }

        const orderId = Number(event.row?.['id']);

        if (!orderId) {
            return;
        }

        this.tableData.update((rows) =>
            rows.map((row) => (Number(row['id']) === orderId ? this.mapOrderToRow(formValue, orderId) : row))
        );

        this.openFeedbackDialog({
            title: 'Pedido atualizado',
            message: 'Pedido alterado com sucesso.',
            tone: 'success'
        });
    }

    private openFeedbackDialog(data: ApiFeedbackDialogData): void {
        this.dialog.open(ApiFeedbackDialogComponent, {
            width: '420px',
            data
        });
    }

    private getNextId(rows: DataTableRow[]): number {
        return rows.reduce((maxId, row) => Math.max(maxId, Number(row['id']) || 0), 0) + 1;
    }

    private mapOrderToRow(order: OrderFormValue, id: number): DataTableRow {
        return {
            id,
            cliente: order.cliente,
            totalPedido: order.totalPedido,
            status: order.status
        };
    }

    private formatCurrency(value: number): string {
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(value);
    }
}