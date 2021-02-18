export const pluckProp = (propertyKey = "", list = []) =>
  list.map((target) => Reflect.get(target, propertyKey))

export const pluckCode = (list = []) => pluckProp("code", list)
