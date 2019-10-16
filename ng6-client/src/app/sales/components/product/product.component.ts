import { Component, OnInit, ViewChild, Query } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, flatMap } from 'rxjs/operators';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { MatSort, PageEvent, Sort } from '@angular/material';
import { QueryParams } from '../../models/query-params';
import { PagedProducts } from '../../models/page-products';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.scss']
})
export class ProductComponent implements OnInit {

  displayedColumns = ['id', 'name', 'productUnit', 'taxRate'];
  dataSource: Product[];
  searchKeyUp = new Subject<string>();

  queryParams: QueryParams = new QueryParams({
    pageIndex: 0,
    pageSize: 5
  });

  constructor(
    private productService: ProductService
  ) {
    const subscription = this.searchKeyUp.pipe(
      debounceTime(500),
      distinctUntilChanged()
    ).subscribe(() => {
      this.load();
    });
  }

  ngOnInit() {
    this.load();
  }

  load() {
    this.productService.loadByPage(this.queryParams).subscribe(result => {
      this.queryParams = Object.assign(this.queryParams, result.pagination);
      this.dataSource = result.value;
    });
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim();
    filterValue = filterValue.toLowerCase();
    this.queryParams.searchTerm = filterValue;
    this.load();
  }

  sortData(sort: Sort) {
    this.queryParams.orderBy = null;
    if (sort.direction) {
      this.queryParams.orderBy = sort.active;
      if (sort.direction === 'desc') {
        this.queryParams.orderBy += ' desc';
      }
    }
    this.load();
  }

  onPaging(pageEvent: PageEvent) {
    this.queryParams.pageIndex = pageEvent.pageIndex;
    this.queryParams.pageSize = pageEvent.pageSize;
    this.queryParams.previousPageIndex = pageEvent.previousPageIndex;
    this.load();
  }
}
