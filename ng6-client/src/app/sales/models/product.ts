import { EntityBase } from './base/entity-base';
import { IHATEOAS } from './interfaces/hateoas';
import { LinkBase } from './base/link-base';

export class Product extends EntityBase implements IHATEOAS {

    name: string;
    fullName: string;
    specification: string;
    productUnit: ProductUnit;
    shelfLife: number;
    equivalentTon: number;
    barcode: number;
    taxRate: number;

    links: LinkBase[];

    constructor(init?: Partial<Product>) {
        super(init);
        Object.assign(this, init);
    }
}

export enum ProductUnit {
    Bag = 1,
    Cup = 2,
    Bowl = 3,
    Box = 4
}
