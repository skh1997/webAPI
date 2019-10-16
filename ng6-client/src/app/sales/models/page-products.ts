import { IHATEOAS } from './interfaces/hateoas';
import { LinkBase } from './base/link-base';
import { Product } from './product';
import { PaginationBase } from './base/pagination-base';

export class PagedProducts implements IHATEOAS {
    pagination: PaginationBase;
    links: LinkBase[];
    value: Product[];

    constructor(init?: Partial<PagedProducts>) {
        Object.assign(this, init);
    }
}
