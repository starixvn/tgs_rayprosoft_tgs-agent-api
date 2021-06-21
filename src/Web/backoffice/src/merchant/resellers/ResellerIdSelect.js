import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import Select from "../../select/Select"
import { useResellerList } from "./client"
import { useTrans } from "../../i18n"

export const ResellerIdSelect = ({ placeholder, ...props }) => {
    let defaultPlaceholder = useResellerPlaceholder()
    placeholder = placeholder ?? defaultPlaceholder

    let { isLoading, data, error } = useResellerList({
        showAll: true,
        sortby: "code"
    })
    let options = data?.items.map((reseller) => ({
        label: reseller.code,
        value: reseller.id
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

const useResellerPlaceholder = () =>
    useTrans({
        zh: "总公司",
        en: "Reseller"
    })

export default ResellerIdSelect
