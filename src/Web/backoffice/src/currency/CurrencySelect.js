import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Select, SelectField } from "../select/Select"
import { useCurrencyList } from "./client"
import { useFormContext } from "react-hook-form"
import { useTrans } from "../i18n"

export const CurrencySelectField = ({
    placeholder,
    placeholderDisabled,
    ...props
}) => {
    let formMethods = useFormContext()

    let defaultPlaceholder = useTrans({
        zh: "选择币种",
        en: "Select Currency"
    })

    let { data, isLoading, error } = useCurrencyList({ showAll: true })

    if (
        isLoading &&
        props?.name &&
        props?.defaultValue &&
        formMethods?.control
    ) {
        return (
            <Controller
                name={props?.name}
                control={formMethods?.control}
                render={() => <Select isLoading={isLoading} options={[]} />}
                defaultValue={props?.defaultValue}
            />
        )
    }

    let currencyOptions =
        data?.items?.map(({ code, ...item }) => ({
            label: code,
            value: code
        })) ?? []

    placeholder = placeholder ?? defaultPlaceholder
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
