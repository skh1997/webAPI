export class LinkBase {
    href: string;
    method: string;
    rel: string;

    constructor(init?: Partial<LinkBase>) {
        Object.assign(this, init);
    }
}
