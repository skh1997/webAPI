export class PaginationBase {
    pageIndex: number;
    pageSize: number;
    orderBy: string;
    count: number;
    maxPageSize: number;
    pageCount: number;

    previousPageIndex?: number;

    constructor(init?: Partial<PaginationBase>) {
        Object.assign(this, init);
    }
}
