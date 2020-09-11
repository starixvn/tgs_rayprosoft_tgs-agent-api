const Create = "create"

const Update = "update"

const Delete = "delete"

//TODO: i18n
export const label = (codes = []) => {
  let [code = ""] = codes
  code = code.toLowerCase()

  switch (code) {
    case Create:
      return "新增部门"
    case Update:
      return "编辑部门"
    case Delete:
      return "删除部门"
    default:
      return ""
  }
}
