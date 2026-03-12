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
    document: [this.data.customer?.document ?? '', [Validators.required, Validators.pattern(/^(\d{11}|\d{3}\.\d{3}\.\d{3}-\d{2})$/)]]
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

  protected onSubmit(): void {
    if (this.customerForm.invalid) {
      this.customerForm.markAllAsTouched();
      return;
    }

    this.dialogRef.close(this.customerForm.getRawValue());
  }
}
