export const mapChecked = (fn = byId, source = [], list = []) =>
  list.map((item) => {
    let checked = !!source.find(fn(item))
    return {
      ...item,
      checked
    }
  })

const byProp = (name) => (a = {}) => (b = {}) => a[name] === b[name]
export const mapCheckedByProp = (name, source = [], list = []) =>
  mapChecked(byProp(name), source, list)

export const mapCheckedById = (source = [], list = []) =>
  mapCheckedByProp("id", source, list)

export const mapCheckedByCode = (source = [], list = []) =>
  mapCheckedByProp("code", source, list)

export const mapCheckedByCultureCode = (source = [], list = []) =>
  mapCheckedByProp("cultureCode", source, list)
