import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { map } from 'rxjs/operators';

import { Product } from '../models/product';
import { QueryParams } from '../models/query-params';
import { HttpResponse } from 'selenium-webdriver/http';
import { PaginationBase } from '../models/base/pagination-base';
import { LinkBase } from '../models/base/link-base';
import { PagedProducts } from '../models/page-products';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  private _baseUrl = '/api/sales/product';

  constructor(
    private http: HttpClient
  ) { }

  loadByPage(params: QueryParams): Observable<PagedProducts> {
    const url = `${this._baseUrl}/paged`;
    return this.http.get(url, { observe: 'response', params: params.asHttpParams() })
      .pipe(
        map(resp => {
          const pagination = JSON.parse(resp.headers.get('X-Pagination')) as PaginationBase;
          return Object.assign({ pagination }, resp.body) as PagedProducts;
        })
      );
  }

}
