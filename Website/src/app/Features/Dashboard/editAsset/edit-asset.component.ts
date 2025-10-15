import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Asset } from '../../../models/asset';
import { AssetType } from '../../../models/assetType';

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

  AssetType = AssetType; // for template access

  constructor() {}

  ngOnInit(): void {}

  addOrUpdate(asset: Asset) {
    this.save.emit();
  }

  hasStockTicker(asset: Asset): boolean {
    return (
      asset.assetType === AssetType.Stocks &&
      asset.ticker &&
      asset.ticker.trim().length > 0
    );
  }

  onDelete() {
    this.delete.emit();
  }
}
