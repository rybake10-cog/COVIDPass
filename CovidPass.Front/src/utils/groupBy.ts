// © 2020 Partners HealthCare Technology & Innovation Department
export const groupBy = <T>(array: T[], selector: (model: T) => any[], keyBuilder?: (keyFields: any[]) => string) => {
  const map = array.reduce((p, c) => {
    const keyFields = selector(c);
    const key = keyBuilder != null ? keyBuilder(keyFields) : keyFields.reduce((p, c) => p + c, '');
    if (p.has(key)) {
      const item = p.get(key);
      if (item != null) item.items.push(c);
    } else {
      p.set(key, { key: keyFields, items: [c] });
    }
    return p;
  }, new Map<string, { key: any[]; items: T[] }>());
  return Array.from(map).map(([, { key, items }]) => ({ key, items }));
};
