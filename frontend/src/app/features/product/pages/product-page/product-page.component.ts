import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ApiResponse } from '@core/models/api-response.model';
import {
    DataTable,
    DataTableColumn,
    DataTableDialogDataBuilder,
    DataTableDialogSubmittedEvent,
    DataTableRow,
    DataTableRowAction
} from '../../../../../styles/components/data-table/data-table';
import {
    ProductFormDialogComponent,
    ProductFormDialogData,
    ProductFormDialogProduct,
    ProductFormValue
} from '../../components/product-form-dialog/product-form-dialog.component';
import {
    ProductApiModel,
    ProductApiService,
    UpsertProductPayload
} from '../../data-access/product-api.service';
import { SidenavResponsive } from '../../../../../styles/components/sidebar/sidenav-responsive';
import {
    ApiFeedbackDialogComponent,
    ApiFeedbackDialogData
} from '../../../../../styles/components/dialog/api-feedback-dialog.component';

@Component({
    selector: 'app-product-page',
    standalone: true,
    imports: [SidenavResponsive, DataTable],
    templateUrl: './product-page.component.html',
    styleUrl: './product-page.component.css',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProductPageComponent implements OnInit {
    private readonly dialog = inject(MatDialog);
    private readonly productApiService = inject(ProductApiService);

    protected readonly productDialogComponent = ProductFormDialogComponent;

    protected readonly rowActions: DataTableRowAction[] = [
        { key: 'edit', label: 'Editar' },
        { key: 'deactivate', label: 'Inativar' },
        { key: 'activate', label: 'Ativar' }
    ];

    protected readonly tableColumns: DataTableColumn[] = [
        { key: 'name', label: 'Nome', sortable: true },
        { key: 'code', label: 'Código', sortable: true },
        {
            key: 'price',
            label: 'Preço',
            sortable: true,
            cell: (row) => this.formatCurrency(Number(row['price'] ?? 0))
        },
        { key: 'qtdStock', label: 'Quantidade em Estoque', sortable: true},
        {
            key: 'active',
            label: 'Ativo',
            sortable: true,
            cell: (row) => (Boolean(row['active']) ? 'Sim' : 'Nao')
        }
    ];

    protected readonly productDialogDataBuilder: DataTableDialogDataBuilder = (row, title) => {
        const dialogData: ProductFormDialogData = {
            title,
            confirmLabel: 'Salvar',
            cancelLabel: 'Cancelar'
        };

        if (row) {
            dialogData.product = {
                name: String(row['name'] ?? ''),
                code: String(row['code'] ?? ''),
                price: Number(row['price'] ?? 0),
                qtdStock: Number(row['qtdStock'] ?? 0),
                active: Boolean(row['active'])
            } satisfies ProductFormDialogProduct;
        }

        return dialogData;
    };

    protected readonly tableData = signal<DataTableRow[]>([]);

    ngOnInit(): void {
        this.loadProducts();
    }

    protected handleDialogSubmitted(event: DataTableDialogSubmittedEvent): void {
        const formValue = event.value as ProductFormValue;

        if (event.mode === 'create') {
            this.createProduct(formValue);
            return;
        }

        const productId = String(event.row?.['id'] ?? '');

        if (!productId) {
            return;
        }

        this.updateProduct(productId, formValue);
    }

    protected handleRowAction(event: { action: DataTableRowAction; row: DataTableRow }): void {
        if (event.action.key !== 'activate' && event.action.key !== 'deactivate') {
            return;
        }

        const productId = String(event.row['id'] ?? '');

        if (!productId) {
            return;
        }

        const shouldActivate = event.action.key === 'activate';
        const payload: UpsertProductPayload = {
            name: String(event.row['name'] ?? ''),
            price: Number(event.row['price'] ?? 0),
            stockQty: Number(event.row['qtdStock'] ?? 0),
            isActive: shouldActivate
        };

        this.productApiService.update(productId, payload).subscribe({
            next: (response) => {
                this.loadProducts();
                this.openFeedbackDialog({
                    title: shouldActivate ? 'Produto ativado' : 'Produto inativado',
                    message: response.mensagem ?? (shouldActivate ? 'Produto ativado com sucesso.' : 'Produto inativado com sucesso.'),
                    tone: 'success'
                });
            },
            error: (error) => {
                this.openFeedbackDialog({
                    title: shouldActivate ? 'Erro ao ativar produto' : 'Erro ao inativar produto',
                    message: this.extractApiErrorMessage(
                        error,
                        shouldActivate ? 'Nao foi possivel ativar o produto.' : 'Nao foi possivel inativar o produto.'
                    ),
                    tone: 'error'
                });
            }
        });
    }

    private openFeedbackDialog(data: ApiFeedbackDialogData): void {
        this.dialog.open(ApiFeedbackDialogComponent, {
            width: '420px',
            data
        });
    }

    private loadProducts(): void {
        this.productApiService.list().subscribe((products) => {
            this.tableData.set(products.map((product) => this.mapProductToRow(product)));
        });
    }

    private createProduct(formValue: ProductFormValue): void {
        this.productApiService.create(this.mapFormValueToPayload(formValue)).subscribe({
            next: (response) => {
                this.loadProducts();
                this.openFeedbackDialog({
                    title: 'Produto cadastrado',
                    message: response.mensagem ?? 'Produto cadastrado com sucesso.',
                    tone: 'success'
                });
            },
            error: (error) => {
                this.openFeedbackDialog({
                    title: 'Erro ao cadastrar produto',
                    message: this.extractApiErrorMessage(error, 'Nao foi possivel cadastrar o produto.'),
                    tone: 'error'
                });
            }
        });
    }

    private updateProduct(productId: string, formValue: ProductFormValue): void {
        this.productApiService.update(productId, this.mapFormValueToPayload(formValue)).subscribe({
            next: (response) => {
                this.loadProducts();
                this.openFeedbackDialog({
                    title: 'Produto atualizado',
                    message: response.mensagem ?? 'Produto alterado com sucesso.',
                    tone: 'success'
                });
            },
            error: (error) => {
                this.openFeedbackDialog({
                    title: 'Erro ao atualizar produto',
                    message: this.extractApiErrorMessage(error, 'Nao foi possivel atualizar o produto.'),
                    tone: 'error'
                });
            }
        });
    }

    private mapFormValueToPayload(product: ProductFormValue): UpsertProductPayload {
        return {
            name: product.name,
            price: product.price,
            stockQty: product.qtdStock,
            isActive: product.active
        };
    }

    private mapProductToRow(product: ProductApiModel): DataTableRow {
        return {
            id: product.id,
            name: product.name,
            code: product.sku,
            price: product.price,
            qtdStock: product.stockQty,
            active: product.isActive
        };
    }

    private extractApiErrorMessage(error: unknown, fallbackMessage: string): string {
        if (error instanceof HttpErrorResponse) {
            const errorBody = error.error as Partial<ApiResponse<unknown>> | string | null;

            if (typeof errorBody === 'string' && errorBody.trim()) {
                return errorBody;
            }

            if (errorBody && typeof errorBody === 'object' && typeof errorBody.mensagem === 'string' && errorBody.mensagem.trim()) {
                return errorBody.mensagem;
            }

            if (typeof error.message === 'string' && error.message.trim()) {
                return error.message;
            }
        }

        return fallbackMessage;
    }

    private formatCurrency(value: number): string {
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(value);
    }
}
