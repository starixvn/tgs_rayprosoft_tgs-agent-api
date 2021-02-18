export const filterByProp = (propertyKey = "", list = []) =>
  list.filter((target) => !!Reflect.get(target, propertyKey))

export const filterBySameProp = ([propertyKey, value], list = []) =>
  list.filter((target) => {
    let targetValue = Reflect.get(target, propertyKey)
    return targetValue === value
  })

export const filterChecked = (list = []) => filterByProp("checked", list)
