import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Asset } from '../../../models/asset';

@Component({
  selector: 'app-edit-asset',
  templateUrl: './edit-asset.component.html',
  styleUrls: [],
})
export class EditAssetComponent implements OnInit {
  @Input() asset!: Asset;
  @Input() isVisible = false;
  @Output() save: EventEmitter<any> = new EventEmitter();
  @Output() delete: EventEmitter<any> = new EventEmitter();

  constructor() {}

  ngOnInit(): void {}

  addOrUpdate(asset: Asset) {
    this.save.emit();
  }

  onDelete() {
    this.delete.emit();
  }
}
