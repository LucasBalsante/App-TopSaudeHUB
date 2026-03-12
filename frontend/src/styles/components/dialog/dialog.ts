import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import {
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle,
} from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

export interface DialogData {
  title: string;
  confirmLabel?: string;
  cancelLabel?: string;
  name?: string;
  animal?: string;
}

@Component({
  selector: 'dialog',
  standalone: true,
  templateUrl: 'dialog.html',
  imports: [
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatButtonModule,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatDialogClose,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Dialog {
  readonly dialogRef = inject(MatDialogRef<Dialog>);
  readonly data = inject<DialogData>(MAT_DIALOG_DATA);
  name = this.data.name ?? '';
  animal = this.data.animal ?? '';

  onNoClick(): void {
    this.dialogRef.close();
  }
}
