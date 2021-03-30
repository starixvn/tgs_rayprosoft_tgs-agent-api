import clone from "clone-deep"

export const lastIndex = (list = []) => (list.length > 0 ? list.length - 1 : 0)

export const isLastIndex = (index = 0, list = []) => index === lastIndex(list)

export const last = (list = []) =>
    list.length > 0 ? list[lastIndex(list)] : undefined

export const cloneLast = (list) => {
    let result = last(list)
    return typeof result == "object" ? clone(result) : result
}

export default last
