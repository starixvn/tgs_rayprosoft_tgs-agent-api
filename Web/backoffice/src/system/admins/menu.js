export const Create = "create"

export const Update = "update"

//TODO: i18n
export const label = (codes = []) => {
  let [code = ""] = codes
  code = code.toLowerCase()

  switch (code) {
    case Create:
      return "新增管理员"
    case Update:
      return "编辑管理员"
    default:
      return ""
  }
}
