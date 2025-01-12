import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { State } from '../../../models/state';
import Utils from '../../../utils/utils';

@Component({
  selector: 'app-edit-state',
  templateUrl: './edit-state.component.html',
  styleUrls: []
})
export class EditStateComponent implements OnInit {

  @Input() state!: State;
  @Input() isVisible = false;
  @Output() save: EventEmitter<any> = new EventEmitter();
  @Output() delete: EventEmitter<any> = new EventEmitter();

  constructor() { }

  ngOnInit(): void {
  }

  addOrUpdate(state: State) {
    this.save.emit();
  }

  onDelete() {
    this.delete.emit();
  }
}
