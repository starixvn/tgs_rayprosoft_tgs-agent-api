export const mapChecked = (fn, source = [], list = []) =>
  list.map((item) => {
    let record = source.find((a) => fn(a, item))
    let checked = !!record
    return {
      ...item,
      checked
    }
  })

export const mapMergeChecked = (predicateFn, mergeFn, source = [], list = []) =>
  list.map((item) => {
    let record = source.find((a) => predicateFn(a, item))
    let checked = !!record
    return {
      ...mergeFn(record, item),
      checked
    }
  })

const byProp = (name) => (a = {}, b = {}) => a[name] === b[name]
export const mapCheckedByProp = (name, source = [], list = []) =>
  mapChecked(byProp(name), source, list)

export const mapCheckedById = (source = [], list = []) =>
  mapCheckedByProp("id", source, list)

export const mapCheckedByCode = (source = [], list = []) =>
  mapCheckedByProp("code", source, list)

export const mapCheckedByCultureCode = (source = [], list = []) =>
  mapCheckedByProp("cultureCode", source, list)
