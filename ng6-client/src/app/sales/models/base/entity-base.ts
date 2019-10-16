export class EntityBase {
    id: number;
    order: number;
    deleted: boolean;

    constructor(init?: Partial<EntityBase>) {
        Object.assign(this, init);
    }
}
