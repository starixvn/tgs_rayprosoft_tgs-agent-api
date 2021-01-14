export const mapChecked = (fn = byId, source = [], list = []) =>
  list.map((item) => {
    let checked = !!source.find(fn(item))
    return {
      ...item,
      checked
    }
  })

const byId = (a = {}) => (b = {}) => a.id === b.id
export const mapCheckedById = (source = [], list = []) =>
  mapChecked(byId, source, list)

const byCode = (a = {}) => (b = {}) => a.code === b.code
export const mapCheckedByCode = (source = [], list = []) =>
  mapChecked(byCode, source, list)
