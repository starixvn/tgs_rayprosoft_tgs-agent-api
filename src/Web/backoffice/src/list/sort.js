export const sortByNumberProp = (propertyKey = "", list = []) =>
  list.sort((a = {}, b = {}) => {
    return Reflect.get(a, propertyKey) - Reflect.get(b, propertyKey)
  })
