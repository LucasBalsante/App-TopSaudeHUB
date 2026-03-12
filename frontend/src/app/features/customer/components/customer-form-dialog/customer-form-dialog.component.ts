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

export interface CustomerFormValue {
  name: string;
  email: string;
  document: string;
}

export interface CustomerFormDialogData {
  title: string;
  confirmLabel?: string;
  cancelLabel?: string;
  customer?: CustomerFormValue;
}

@Component({
  selector: 'app-customer-form-dialog',
  standalone: true,
  templateUrl: './customer-form-dialog.component.html',
  styleUrl: './customer-form-dialog.component.css',
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
export class CustomerFormDialogComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<CustomerFormDialogComponent, CustomerFormValue>);
  protected readonly data = inject<CustomerFormDialogData>(MAT_DIALOG_DATA);

  protected readonly customerForm = this.formBuilder.nonNullable.group({
    name: [this.data.customer?.name ?? '', [Validators.required, Validators.minLength(3)]],
    email: [this.data.customer?.email ?? '', [Validators.required, Validators.email]],
    document: [this.formatCpf(this.data.customer?.document ?? ''), [Validators.required, Validators.pattern(/^(\d{3}\.\d{3}\.\d{3}-\d{2})$/)]]
  });

  protected get nameControl() {
    return this.customerForm.controls.name;
  }

  protected get emailControl() {
    return this.customerForm.controls.email;
  }

  protected get documentControl() {
    return this.customerForm.controls.document;
  }

  protected onCancel(): void {
    this.dialogRef.close();
  }

  protected onDocumentInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const formattedValue = this.formatCpf(input.value);

    this.documentControl.setValue(formattedValue, { emitEvent: false });
  }

  protected onSubmit(): void {
    if (this.customerForm.invalid) {
      this.customerForm.markAllAsTouched();
      return;
    }

    this.dialogRef.close({
      ...this.customerForm.getRawValue(),
      document: this.extractDigits(this.documentControl.getRawValue())
    });
  }

  private formatCpf(value: string): string {
    const digits = this.extractDigits(value).slice(0, 11);

    if (digits.length <= 3) {
      return digits;
    }

    if (digits.length <= 6) {
      return `${digits.slice(0, 3)}.${digits.slice(3)}`;
    }

    if (digits.length <= 9) {
      return `${digits.slice(0, 3)}.${digits.slice(3, 6)}.${digits.slice(6)}`;
    }

    return `${digits.slice(0, 3)}.${digits.slice(3, 6)}.${digits.slice(6, 9)}-${digits.slice(9, 11)}`;
  }

  private extractDigits(value: string): string {
    return value.replace(/\D/g, '');
  }
}
