import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
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

export interface OrderFormValue {
  cliente: string;
  totalPedido: number;
  status: string;
}

export interface OrderFormDialogData {
  title: string;
  confirmLabel?: string;
  cancelLabel?: string;
  order?: OrderFormValue;
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
    MatInputModule
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class OrderFormDialogComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<OrderFormDialogComponent, OrderFormValue>);
  protected readonly data = inject<OrderFormDialogData>(MAT_DIALOG_DATA);

  protected readonly orderForm = this.formBuilder.nonNullable.group({
    cliente: [this.data.order?.cliente ?? '', [Validators.required, Validators.minLength(3)]],
    totalPedido: [this.data.order?.totalPedido ?? 0, [Validators.required, Validators.min(0.01)]],
    status: [this.data.order?.status ?? '', [Validators.required, Validators.minLength(3)]]
  });

  protected get clienteControl() {
    return this.orderForm.controls.cliente;
  }

  protected get totalPedidoControl() {
    return this.orderForm.controls.totalPedido;
  }

  protected get statusControl() {
    return this.orderForm.controls.status;
  }

  protected onCancel(): void {
    this.dialogRef.close();
  }

  protected onSubmit(): void {
    if (this.orderForm.invalid) {
      this.orderForm.markAllAsTouched();
      return;
    }

    this.dialogRef.close(this.orderForm.getRawValue());
  }
}