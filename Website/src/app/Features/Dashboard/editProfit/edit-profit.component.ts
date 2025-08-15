import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Profit } from '../../../models/profit';

@Component({
  selector: 'app-edit-profit',
  templateUrl: './edit-profit.component.html',
  styleUrls: []
})
export class EditProfitComponent implements OnInit {

  @Input() profit!: Profit;
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
