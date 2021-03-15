import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import Select from "../../select/Select"
import { useBrandList } from "./client"
import { useTrans } from "../../i18n"

export const BrandIdSelect = ({
    licenseeId = "",
    placeholderDisabled,
    ...props
}) => {
    let { data, isLoading } = useBrandList({ showAll: true, licenseeId })

    let placeholder = useBrandPlaceholder()
    let placeholderOption = {
        label: placeholder,
        value: "",
        disabled: placeholderDisabled
    }

    if (isLoading) {
        return (
            <Select
                isLoading={isLoading}
                options={[placeholderOption]}
                {...props}
            />
        )
    }

    let options =
        data?.items.map((brand) => ({
            label: brand.code,
            value: brand.id
        })) ?? []
    return <Select options={[placeholderOption, ...options]} {...props} />
}

const useBrandPlaceholder = () =>
    useTrans({
        zh: "选择品牌",
        en: "Select Brand"
    })
export default BrandIdSelect
