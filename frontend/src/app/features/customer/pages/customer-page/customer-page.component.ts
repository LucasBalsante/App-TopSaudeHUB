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
import { SidenavResponsive } from '../../../../../styles/components/sidebar/sidenav-responsive';
import {
  CustomerFormDialogComponent,
  CustomerFormDialogData,
  CustomerFormValue
} from '../../components/customer-form-dialog/customer-form-dialog.component';
import { CustomerApiModel, CustomerApiService } from '../../data-access/customer-api.service';
import {
  ApiFeedbackDialogComponent,
  ApiFeedbackDialogData
} from '../../../../../styles/components/dialog/api-feedback-dialog.component';

@Component({
  selector: 'app-customer-page',
  standalone: true,
  imports: [SidenavResponsive, DataTable],
  templateUrl: './customer-page.component.html',
  styleUrl: './customer-page.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CustomerPageComponent implements OnInit {
  private readonly customerApiService = inject(CustomerApiService);
  private readonly dialog = inject(MatDialog);

  protected readonly customerDialogComponent = CustomerFormDialogComponent;

  protected readonly rowActions: DataTableRowAction[] = [
    { key: 'edit', label: 'Editar' }
  ];

  protected readonly customerDialogDataBuilder: DataTableDialogDataBuilder = (row, title) => {
    const dialogData: CustomerFormDialogData = {
      title,
      confirmLabel: 'Salvar',
      cancelLabel: 'Cancelar'
    };

    if (row) {
      dialogData.customer = {
        name: String(row['name'] ?? ''),
        email: String(row['email'] ?? ''),
        document: String(row['document'] ?? '')
      };
    }

    return dialogData;
  };

  protected readonly tableColumns: DataTableColumn[] = [
    { key: 'name', label: 'Nome', sortable: true },
    { key: 'email', label: 'Email', sortable: true },
    { key: 'document', label: 'Documento', sortable: true }
  ];

  protected readonly tableData = signal<DataTableRow[]>([]);

  ngOnInit(): void {
    this.loadCustomers();
  }

  protected handleDialogSubmitted(event: DataTableDialogSubmittedEvent): void {
    const formValue = event.value as CustomerFormValue;

    if (event.mode === 'create') {
      this.createCustomer(formValue);
      return;
    }

    const customerId = String(event.row?.['id'] ?? '');

    if (!customerId) {
      return;
    }

    this.updateCustomer(customerId, formValue);
  }

  private loadCustomers(): void {
    this.customerApiService.list().subscribe((customers) => {
      this.tableData.set(customers.map((customer) => this.mapCustomerToRow(customer)));
    });
  }

  private createCustomer(formValue: CustomerFormValue): void {
    this.customerApiService.create(formValue).subscribe({
      next: (response) => {
        this.loadCustomers();
        this.openFeedbackDialog({
          title: 'Cliente cadastrado',
          message: response.mensagem ?? 'Cliente cadastrado com sucesso.',
          tone: 'success'
        });
      },
      error: (error) => {
        this.openFeedbackDialog({
          title: 'Erro ao cadastrar cliente',
          message: this.extractApiErrorMessage(error, 'Nao foi possivel cadastrar o cliente.'),
          tone: 'error'
        });
      }
    });
  }

  private updateCustomer(customerId: string, formValue: CustomerFormValue): void {
    this.customerApiService.update(customerId, formValue).subscribe({
      next: (response) => {
        this.loadCustomers();
        this.openFeedbackDialog({
          title: 'Cliente atualizado',
          message: response.mensagem ?? 'Cliente alterado com sucesso.',
          tone: 'success'
        });
      },
      error: (error) => {
        this.openFeedbackDialog({
          title: 'Erro ao atualizar cliente',
          message: this.extractApiErrorMessage(error, 'Nao foi possivel atualizar o cliente.'),
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

  private mapCustomerToRow(customer: CustomerApiModel): DataTableRow {
    return {
      id: customer.id,
      name: customer.name,
      email: customer.email,
      document: customer.document
    };
  }
}
