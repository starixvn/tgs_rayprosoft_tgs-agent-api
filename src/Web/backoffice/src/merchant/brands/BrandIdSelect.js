import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import Select from "../../select/Select"
import { useBrandList } from "./client"
import { useTrans } from "../../i18n"

export const BrandIdSelect = ({ placeholder, licenseeId, ...props }) => {
    let defaultPlaceholder = useBrandPlaceholder()
    placeholder = placeholder ?? defaultPlaceholder

    let { isLoading, data, error } = useBrandList({ showAll: true, licenseeId })
    let options = data?.items.map((brand) => ({
        label: brand.code,
        value: brand.id
    }))
    return (
        <Select
            placeholder={placeholder}
            isLoading={isLoading}
            options={options}
            {...props}
        />
    )
}

const useBrandPlaceholder = () =>
    useTrans({
        zh: "选择品牌",
        en: "Select Brand"
    })
export default BrandIdSelect
