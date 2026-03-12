import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import {
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogTitle
} from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';

export interface ApiFeedbackDialogData {
  title: string;
  message: string;
  tone: 'success' | 'error';
  closeLabel?: string;
}

@Component({
  selector: 'app-api-feedback-dialog',
  standalone: true,
  templateUrl: './api-feedback-dialog.component.html',
  styleUrl: './api-feedback-dialog.component.css',
  imports: [MatButtonModule, MatIconModule, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ApiFeedbackDialogComponent {
  protected readonly data = inject<ApiFeedbackDialogData>(MAT_DIALOG_DATA);

  protected get iconName(): string {
    return this.data.tone === 'success' ? 'check_circle' : 'error';
  }
}