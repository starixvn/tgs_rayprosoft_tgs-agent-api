import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import Select from "../../select/Select"
import { useBrandList } from "./client"
import { useTrans } from "../../i18n"
import { Controller, useFormContext } from "react-hook-form"

export const BrandIdSelect = ({
    licenseeId = "",
    placeholderDisabled,
    ...props
}) => {
    let formMethods = useFormContext()
    let { data, isLoading } = useBrandList({ showAll: true, licenseeId })

    let placeholder = useBrandPlaceholder()
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
