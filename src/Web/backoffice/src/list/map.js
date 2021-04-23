import clone from "clone-deep"

export const mapOmit = (props = [], list = []) => {
    return list.map((item) => {
        let cloned = clone(item)
        props.map((prop) => Reflect.deleteProperty(cloned, prop))
        return item
    })
}
