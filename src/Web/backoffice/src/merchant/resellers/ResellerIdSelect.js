import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import Select from "../../select/Select"
import { useResellerList } from "./client"
import { useTrans } from "../../i18n"
import { Controller, useFormContext } from "react-hook-form"

export const ResellerIdSelect = ({ placeholderDisabled = false, ...props }) => {
    let formMethods = useFormContext()
    let { data, isLoading } = useResellerList({ showAll: true })

    let placeholder = useResellerPlaceholder()
    let placeholderOption = {
        label: placeholder,
        value: "",
        disabled: placeholderDisabled
    }

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

    let options =
        data?.items.map((reseller) => ({
            label: reseller.code,
            value: reseller.id
        })) ?? []

    return <Select options={[placeholderOption, ...options]} {...props} />
}

const useResellerPlaceholder = () =>
    useTrans({
        zh: "选择总公司",
        en: "Select Reseller"
    })

export default ResellerIdSelect
