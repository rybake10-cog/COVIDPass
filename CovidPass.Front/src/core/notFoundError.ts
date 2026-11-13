// © 2020 Partners HealthCare Technology & Innovation Department
export class NotFoundError extends Error {
  public constructor(message?: string) {
    super(message);
    Object.setPrototypeOf(this, new.target.prototype);
    this.name = NotFoundError.name;
  }
}
