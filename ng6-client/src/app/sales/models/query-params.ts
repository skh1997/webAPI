import { HttpParams } from '@angular/common/http';
import { PaginationBase } from './base/pagination-base';

export class QueryParams extends PaginationBase {
    searchTerm: string;
    fields: string;

    constructor(init?: Partial<QueryParams>) {
        super(init);
        Object.assign(this, init);
    }

    asHttpParams() {
        return new HttpParams()
        .set('pageIndex', this.pageIndex.toString())
        .set('pageSize', this.pageSize.toString())
        .set('orderBy', this.orderBy ? this.orderBy : '')
        .set('searchTerm', this.searchTerm ? this.searchTerm : '')
        .set('fields', this.fields ? this.fields : '');
    }
}
