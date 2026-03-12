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

export interface ProductFormValue {
  name: string;
  price: number;
  qtdStock: number;
  active: boolean;
}

export interface ProductFormDialogProduct extends ProductFormValue {
  code?: string;
}

export interface ProductFormDialogData {
  title: string;
  confirmLabel?: string;
  cancelLabel?: string;
  product?: ProductFormDialogProduct;
}

@Component({
  selector: 'app-product-form-dialog',
  standalone: true,
  templateUrl: './product-form-dialog.component.html',
  styleUrl: './product-form-dialog.component.css',
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
export class ProductFormDialogComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<ProductFormDialogComponent, ProductFormValue>);
  protected readonly data = inject<ProductFormDialogData>(MAT_DIALOG_DATA);
  protected readonly productCode = this.data.product?.code ?? '';
  protected readonly isEditMode = Boolean(this.data.product?.code);

  protected readonly productForm = this.formBuilder.nonNullable.group({
    name: [this.data.product?.name ?? '', [Validators.required, Validators.minLength(3)]],
    price: [this.data.product?.price ?? 0, [Validators.required, Validators.min(0.01)]],
    qtdStock: [this.data.product?.qtdStock ?? 0, [Validators.required, Validators.min(0)]]
  });

  protected get nameControl() {
    return this.productForm.controls.name;
  }

  protected get priceControl() {
    return this.productForm.controls.price;
  }

  protected get qtdStockControl() {
    return this.productForm.controls.qtdStock;
  }

  protected onCancel(): void {
    this.dialogRef.close();
  }

  protected onSubmit(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    this.dialogRef.close({
      ...this.productForm.getRawValue(),
      active: this.data.product?.active ?? true
    });
  }

  protected handleEnterSubmit(event: Event): void {
    event.preventDefault();
    this.onSubmit();
  }
}