import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Expense } from '../../../models/expense';

@Component({
  selector: 'app-edit-expense',
  templateUrl: './edit-expense.component.html',
  styleUrls: []
})
export class EditExpenseComponent implements OnInit {

  @Input() expense!: Expense;
  @Input() isVisible = false;
  @Output() save: EventEmitter<any> = new EventEmitter();
  @Output() delete: EventEmitter<any> = new EventEmitter();

  constructor() { }

  ngOnInit(): void {
  }

  addOrUpdate() {
    this.save.emit();
  }

  onDelete() {
    this.delete.emit();
  }
}
