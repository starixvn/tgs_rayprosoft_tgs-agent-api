import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Select, SelectField } from "../select/Select"
import { useCurrencyList } from "./client"

export const CurrencySelectField = ({
    placeholder,
    placeholderDisabled,
    ...props
}) => {
    let defaultPlaceholder = useTrans({
        zh: "选择币种",
        en: "Select Currency"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let { data, isLoading, error } = useCurrencyList({ showAll: true })
    let currencyOptions =
        data?.items?.map(({ code, ...item }) => ({
            label: item.name,
            value: code
        })) ?? []

    let options =
        typeof placeholder === "string"
            ? [
                  {
                      label: placeholder,
                      value: "",
                      disabled: placeholderDisabled
                  },
                  ...currencyOptions
              ]
            : currencyOptions

    return <SelectField isLoading={isLoading} options={options} {...props} />
}
