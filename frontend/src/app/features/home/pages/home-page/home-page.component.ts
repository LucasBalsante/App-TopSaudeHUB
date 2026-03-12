import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
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
    OrderFormDialogOrder,
    OrderFormValue
} from '../../components/order-form-dialog/order-form-dialog.component';
import { SidenavResponsive } from '../../../../../styles/components/sidebar/sidenav-responsive';
import {
    ApiFeedbackDialogComponent,
    ApiFeedbackDialogData
} from '../../../../../styles/components/dialog/api-feedback-dialog.component';
import {
    OrderApiListModel,
    OrderApiModel,
    OrderApiService,
    OrderMutationResponse,
    UpsertOrderPayload
} from '../../data-access/order-api.service';

interface OrderItemSummary {
    productId: string;
    productName: string;
    quantity: number;
    unitPrice: number;
    lineTotal: number;
}

interface SelectedOrderSummary {
    id: string;
    customerName: string;
    totalAmount: number;
    items: OrderItemSummary[];
}

@Component({
    selector: 'app-home-page',
    standalone: true,
    imports: [SidenavResponsive, DataTable],
    templateUrl: './home-page.component.html',
    styleUrl: './home-page.component.css',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomePageComponent implements OnInit {
    private readonly dialog = inject(MatDialog);
    private readonly orderApiService = inject(OrderApiService);

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
        {
            key: 'status',
            label: 'Status',
            sortable: true,
            cell: (row) => this.formatOrderStatus(String(row['status'] ?? ''))
        },
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
                customerId: String(row['customerId'] ?? ''),
                products: this.parseOrderProducts(row['products']),
                totalPedido: Number(row['totalPedido'] ?? 0),
                status: String(row['status'] ?? '')
            } satisfies OrderFormDialogOrder;
        }

        return dialogData;
    };

    protected readonly tableData = signal<DataTableRow[]>([]);
    protected readonly selectedOrders = signal<SelectedOrderSummary[]>([]);

    ngOnInit(): void {
        this.loadOrders();
    }

    protected handleDialogSubmitted(event: DataTableDialogSubmittedEvent): void {
        const formValue = event.value as OrderFormValue;

        if (event.mode === 'create') {
            this.createOrder(formValue);
            return;
        }

        const orderId = String(event.row?.['id'] ?? '');

        if (!orderId) {
            return;
        }

        this.updateOrder(orderId, formValue);
    }

    protected handleSelectionChanged(rows: DataTableRow[]): void {
        this.selectedOrders.set(
            rows.map((row) => ({
                id: String(row['id'] ?? ''),
                customerName: String(row['cliente'] ?? ''),
                totalAmount: Number(row['totalPedido'] ?? 0),
                items: this.parseOrderItems(row['orderItems'])
            }))
        );
    }

    protected handleRowAction(event: { action: DataTableRowAction; row: DataTableRow }): void {
        if (event.action.key !== 'pay-order' && event.action.key !== 'cancel-order') {
            return;
        }

        const orderId = String(event.row['id'] ?? '');
        const customerId = String(event.row['customerId'] ?? '');
        const products = this.parseOrderProducts(event.row['products']);

        if (!orderId || !customerId || !products.length) {
            return;
        }

        const nextStatus = event.action.key === 'pay-order' ? 1 : 2;

        this.orderApiService.update(orderId, {
            customerId,
            Items: products.map((product) => ({
                productId: product.productId,
                Quantity: product.quantity
            })),
            status: nextStatus
        }).subscribe({
            next: (response) => {
                this.loadOrders();

                this.openFeedbackDialog({
                    title: nextStatus === 1 ? 'Pedido pago' : 'Pedido cancelado',
                    message: this.getResponseMessage(
                        response,
                        nextStatus === 1 ? 'Pedido pago com sucesso.' : 'Pedido cancelado com sucesso.'
                    ),
                    tone: 'success'
                });
            },
            error: (error) => {
                this.openFeedbackDialog({
                    title: nextStatus === 1 ? 'Erro ao pagar pedido' : 'Erro ao cancelar pedido',
                    message: this.extractApiErrorMessage(
                        error,
                        nextStatus === 1 ? 'Nao foi possivel pagar o pedido.' : 'Nao foi possivel cancelar o pedido.'
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

    private loadOrders(): void {
        this.orderApiService.list().subscribe({
            next: (orders) => {
                this.tableData.set(orders.map((order) => this.mapOrderApiToRow(order)));
                this.selectedOrders.set([]);
            },
            error: (error) => {
                this.openFeedbackDialog({
                    title: 'Erro ao carregar pedidos',
                    message: this.extractApiErrorMessage(error, 'Nao foi possivel carregar os pedidos.'),
                    tone: 'error'
                });
            }
        });
    }

    private createOrder(formValue: OrderFormValue): void {
        this.orderApiService.create(this.mapFormValueToPayload(formValue)).subscribe({
            next: (response) => {
                this.loadOrders();

                this.openFeedbackDialog({
                    title: 'Pedido cadastrado',
                    message: this.getResponseMessage(response, 'Pedido cadastrado com sucesso.'),
                    tone: 'success'
                });
            },
            error: (error) => {
                this.openFeedbackDialog({
                    title: 'Erro ao cadastrar pedido',
                    message: this.extractApiErrorMessage(error, 'Nao foi possivel cadastrar o pedido.'),
                    tone: 'error'
                });
            }
        });
    }

    private updateOrder(orderId: string, formValue: OrderFormValue): void {
        this.orderApiService.update(orderId, this.mapFormValueToPayload(formValue)).subscribe({
            next: (response) => {
                this.loadOrders();

                this.openFeedbackDialog({
                    title: 'Pedido atualizado',
                    message: this.getResponseMessage(response, 'Pedido alterado com sucesso.'),
                    tone: 'success'
                });
            },
            error: (error) => {
                this.openFeedbackDialog({
                    title: 'Erro ao atualizar pedido',
                    message: this.extractApiErrorMessage(error, 'Nao foi possivel atualizar o pedido.'),
                    tone: 'error'
                });
            }
        });
    }

    private mapOrderApiToRow(order: OrderApiListModel): DataTableRow {
        return {
            id: order.id,
            customerId: order.customer.id,
            cliente: order.customer.nome,
            products: JSON.stringify(
                order.items.map((item) => ({
                    productId: item.product.id,
                    quantity: item.quantidade
                }))
            ),
            orderItems: JSON.stringify(
                order.items.map((item) => ({
                    productId: item.product.id,
                    productName: item.product.name,
                    quantity: item.quantidade,
                    unitPrice: item.product.price,
                    lineTotal: item.lineTotal
                }))
            ),
            totalPedido: order.totalAmount,
            status: order.status
        };
    }

    private mapFormValueToPayload(order: OrderFormValue): UpsertOrderPayload {
        return {
            customerId: order.customerId,
            Items: order.products.map((product) => ({
                productId: product.productId,
                Quantity: product.quantity
            }))
        };
    }

    private parseOrderProducts(value: DataTableRow[string]): OrderFormValue['products'] {
        if (typeof value !== 'string' || !value.trim()) {
            return [];
        }

        try {
            const parsed = JSON.parse(value);

            if (!Array.isArray(parsed)) {
                return [];
            }

            return parsed.filter((item): item is OrderFormValue['products'][number] => {
                if (!item || typeof item !== 'object') {
                    return false;
                }

                const product = item as Partial<OrderFormValue['products'][number]>;
                return typeof product.productId === 'string' && typeof product.quantity === 'number';
            });
        } catch {
            return [];
        }
    }

    private parseOrderItems(value: DataTableRow[string]): OrderItemSummary[] {
        if (typeof value !== 'string' || !value.trim()) {
            return [];
        }

        try {
            const parsed = JSON.parse(value);

            if (!Array.isArray(parsed)) {
                return [];
            }

            return parsed.filter((item): item is OrderItemSummary => {
                if (!item || typeof item !== 'object') {
                    return false;
                }

                const orderItem = item as Partial<OrderItemSummary>;
                return typeof orderItem.productId === 'string'
                    && typeof orderItem.productName === 'string'
                    && typeof orderItem.quantity === 'number'
                    && typeof orderItem.unitPrice === 'number'
                    && typeof orderItem.lineTotal === 'number';
            });
        } catch {
            return [];
        }
    }

    private extractApiErrorMessage(error: unknown, fallbackMessage: string): string {
        if (error instanceof HttpErrorResponse) {
            const errorBody = error.error as Partial<OrderMutationResponse<unknown>> | string | null;

            if (typeof errorBody === 'string' && errorBody.trim()) {
                return errorBody;
            }

            if (errorBody && typeof errorBody === 'object') {
                if (typeof errorBody.message === 'string' && errorBody.message.trim()) {
                    return errorBody.message;
                }

                if (typeof errorBody.mensagem === 'string' && errorBody.mensagem.trim()) {
                    return errorBody.mensagem;
                }
            }

            if (typeof error.message === 'string' && error.message.trim()) {
                return error.message;
            }
        }

        return fallbackMessage;
    }

    protected formatCurrency(value: number): string {
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(value);
    }

    private getResponseMessage(response: OrderMutationResponse<OrderApiModel>, fallbackMessage: string): string {
        if (typeof response.message === 'string' && response.message.trim()) {
            return response.message;
        }

        if (typeof response.mensagem === 'string' && response.mensagem.trim()) {
            return response.mensagem;
        }

        return fallbackMessage;
    }

    private formatOrderStatus(status: string): string {
        switch (status) {
            case '0':
                return 'Criado';
            case '1':
                return 'Pago';
            case '2':
                return 'Cancelado';
            default:
                return status
                    .toLowerCase()
                    .replace(/_/g, ' ')
                    .replace(/\b\w/g, (character) => character.toUpperCase());
        }
    }
}