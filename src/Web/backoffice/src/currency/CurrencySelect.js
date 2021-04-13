import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Select, SelectField } from "../select/Select"
import { useCurrencyList } from "./client"
import { useTrans } from "../i18n"

export const CurrencySelectField = ({ placeholder, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "选择币种",
        en: "Select Currency"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let { isLoading, data, error } = useCurrencyList({ showAll: true })
    let options = data?.items?.map(({ code, ...item }) => ({
        label: code,
        value: code
    }))

    return (
        <SelectField
            placeholder={placeholder}
            isLoading={isLoading}
            options={options}
            {...props}
        />
    )
}
