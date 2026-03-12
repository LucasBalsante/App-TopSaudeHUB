import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import {
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle
} from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { CustomerApiModel, CustomerApiService } from '../../../customer/data-access/customer-api.service';
import { ProductApiModel, ProductApiService } from '../../../product/data-access/product-api.service';

export interface OrderSelectedProductValue {
  productId: string;
  quantity: number;
}

export interface OrderFormValue {
  customerId: string;
  customerName: string;
  products: OrderSelectedProductValue[];
  totalPedido: number;
}

export interface OrderFormDialogOrder {
  cliente: string;
  customerId?: string;
  products?: OrderSelectedProductValue[];
  totalPedido?: number;
  status?: string;
}

export interface OrderFormDialogData {
  title: string;
  confirmLabel?: string;
  cancelLabel?: string;
  order?: OrderFormDialogOrder;
}

interface SelectedOrderProduct extends ProductApiModel {
  quantity: number;
}

@Component({
  selector: 'app-order-form-dialog',
  standalone: true,
  templateUrl: './order-form-dialog.component.html',
  styleUrl: './order-form-dialog.component.css',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatOptionModule
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class OrderFormDialogComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<OrderFormDialogComponent, OrderFormValue>);
  protected readonly data = inject<OrderFormDialogData>(MAT_DIALOG_DATA);
  private readonly customerApiService = inject(CustomerApiService);
  private readonly productApiService = inject(ProductApiService);
  protected readonly customerOptions = signal<CustomerApiModel[]>([]);
  protected readonly productOptions = signal<ProductApiModel[]>([]);
  protected readonly selectedProducts = signal<SelectedOrderProduct[]>([]);
  protected readonly editingProductId = signal<string | null>(null);

  protected readonly orderForm = this.formBuilder.nonNullable.group({
    cliente: ['', [Validators.required]],
    productId: [''],
    quantity: [1, [Validators.required, Validators.min(1)]]
  });

  ngOnInit(): void {
    this.customerApiService.list().subscribe((customers) => {
      this.customerOptions.set(customers);

      const selectedCustomerId = this.resolveCustomerId(customers);

      if (selectedCustomerId) {
        this.clienteControl.setValue(selectedCustomerId);
      }
    });

    this.productApiService.list().subscribe((products) => {
      this.productOptions.set(products);

      const orderProducts = this.data.order?.products ?? [];

      if (!orderProducts.length) {
        return;
      }

      this.selectedProducts.set(
        orderProducts
          .map((selectedProduct) => {
            const product = products.find((item) => item.id === selectedProduct.productId);

            if (!product) {
              return null;
            }

            return {
              ...product,
              quantity: selectedProduct.quantity
            };
          })
          .filter((product): product is SelectedOrderProduct => product !== null)
      );
    });
  }

  protected get clienteControl() {
    return this.orderForm.controls.cliente;
  }

  protected get productIdControl() {
    return this.orderForm.controls.productId;
  }

  protected get quantityControl() {
    return this.orderForm.controls.quantity;
  }

  protected addProduct(): void {
    const selectedProductId = this.productIdControl.getRawValue();
    const quantity = Number(this.quantityControl.getRawValue());
    const editingProductId = this.editingProductId();

    if (!selectedProductId || !quantity || quantity < 1) {
      return;
    }

    const product = this.productOptions().find((item) => item.id === selectedProductId);

    if (!product) {
      return;
    }

    if (!editingProductId && this.selectedProducts().some((item) => item.id === product.id)) {
      this.quantityControl.setValue(1);
      this.productIdControl.setValue('');
      return;
    }

    if (editingProductId) {
      this.selectedProducts.update((items) =>
        items.map((item) => (item.id === editingProductId ? { ...product, quantity } : item))
      );
      this.editingProductId.set(null);
    } else {
      this.selectedProducts.update((items) => [...items, { ...product, quantity }]);
    }

    this.productIdControl.setValue('');
    this.quantityControl.setValue(1);
  }

  protected editProduct(productId: string): void {
    const product = this.selectedProducts().find((item) => item.id === productId);

    if (!product) {
      return;
    }

    this.productIdControl.setValue(product.id);
    this.quantityControl.setValue(product.quantity);
    this.editingProductId.set(product.id);
  }

  protected removeProduct(productId: string): void {
    this.selectedProducts.update((items) => items.filter((item) => item.id !== productId));

    if (this.editingProductId() === productId) {
      this.editingProductId.set(null);
      this.productIdControl.setValue('');
      this.quantityControl.setValue(1);
    }
  }

  protected onCancel(): void {
    this.dialogRef.close();
  }

  protected onSubmit(): void {
    if (this.orderForm.invalid) {
      this.orderForm.markAllAsTouched();
      return;
    }

    this.dialogRef.close({
      customerId: this.clienteControl.getRawValue(),
      customerName: this.getSelectedCustomerName(),
      products: this.selectedProducts().map((product) => ({
        productId: product.id,
        quantity: product.quantity
      })),
      totalPedido: this.getOrderTotal()
    });
  }

  protected formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  protected getProductTotal(product: SelectedOrderProduct): number {
    return product.price * product.quantity;
  }

  protected getOrderTotal(): number {
    const selectedProducts = this.selectedProducts();

    if (!selectedProducts.length) {
      return this.data.order?.totalPedido ?? 0;
    }

    return selectedProducts.reduce((total, product) => total + this.getProductTotal(product), 0);
  }

  protected hasSelectedProducts(): boolean {
    return this.selectedProducts().length > 0;
  }

  private getSelectedCustomerName(): string {
    const customerId = this.clienteControl.getRawValue();

    if (!customerId) {
      return '';
    }

    return this.customerOptions().find((customer) => customer.id === customerId)?.name ?? '';
  }

  private resolveCustomerId(customers: CustomerApiModel[]): string {
    const customerId = this.data.order?.customerId ?? '';

    if (customerId && customers.some((customer) => customer.id === customerId)) {
      return customerId;
    }

    const customerName = this.data.order?.cliente ?? '';
    return customers.find((customer) => customer.name === customerName)?.id ?? '';
  }
}