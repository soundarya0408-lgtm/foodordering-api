import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomerService, Customer } from '../../../core/services/customer';
import { AuthService } from '../../../core/services/auth';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  selector: 'app-customer-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatInputModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './customer-list.html',
  styleUrl: './customer-list.scss'
})
export class CustomerList implements OnInit {

  customers: Customer[] = [];
  displayedColumns = ['name', 'mobileNumber', 'email', 'address', 'actions'];
  isLoading = false;
  showForm = false;
  isEditing = false;
  selectedId: number | null = null;

  customerForm: FormGroup;

  constructor(
    private customerService: CustomerService,
    private authService: AuthService,
    private fb: FormBuilder,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef
  ) {
    this.customerForm = this.fb.group({
      name:         ['', Validators.required],
      mobileNumber: ['', [Validators.required, Validators.pattern('^[0-9]{10}$')]],
      email:        ['', Validators.email],
      address:      [''],
      dob:          [''],
      createdBy:    ['']
    });
  }

  ngOnInit() {
    this.loadCustomers();
  }

  loadCustomers() {
    this.isLoading = true;
    this.customerService.getAll().subscribe({
      next: (data) => { this.customers = data; this.isLoading = false; this.cdr.detectChanges(); },
      error: () => { this.isLoading = false; this.cdr.detectChanges(); this.showSnack('Failed to load customers!');   }
    });
  }

  openAddForm() {
    this.customerForm.reset();
    this.isEditing = false;
    this.selectedId = null;
    this.showForm = true;
  }

  openEditForm(customer: Customer) {
    this.isEditing = true;
    this.selectedId = customer.id;
    this.customerForm.patchValue(customer);
    this.showForm = true;
  }

  saveCustomer() {
    if (this.customerForm.invalid) return;

    const data = this.customerForm.value;

    if (this.isEditing && this.selectedId) {
      this.customerService.update(this.selectedId, data).subscribe({
        next: () => { this.showSnack('Customer updated!'); this.loadCustomers(); this.showForm = false;  this.cdr.detectChanges(); },
        error: () => {this.showSnack('Update failed!'); this.cdr.detectChanges(); }
      });
    } else {
      this.customerService.create(data).subscribe({
        next: () => { this.showSnack('Customer created!'); this.loadCustomers(); this.showForm = false;  this.cdr.detectChanges(); },
        error: () => {this.showSnack('Create failed!'); this.cdr.detectChanges(); }
      });
    }
  }

  deleteCustomer(id: number) {
    if (!confirm('Delete this customer?')) return;
    this.customerService.delete(id).subscribe({
      next: () => { this.showSnack('Customer deleted!'); this.loadCustomers();  this.cdr.detectChanges(); },
      error: () => {this.showSnack('Delete failed!'); this.cdr.detectChanges(); }
    });
  }

  logout() {
    this.authService.logout();
  }

  showSnack(message: string) {
    this.snackBar.open(message, 'Close', { duration: 3000 });
  }
}
