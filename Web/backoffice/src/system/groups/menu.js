export const Code = "groups"

export const Create = "create"

export const Update = "update"

export const Delete = "delete"

export const Copy = "copy"

//TODO: i18n
export const label = (codes = []) => {
  let [code = ""] = codes
  code = code.toLowerCase()

  switch (code) {
    case Create:
      return "新增管理组"
    case Update:
      return "编辑管理组"
    case Delete:
      return "删除管理组"
    case Copy:
      return "复制管理组"
    default:
      return ""
  }
}
