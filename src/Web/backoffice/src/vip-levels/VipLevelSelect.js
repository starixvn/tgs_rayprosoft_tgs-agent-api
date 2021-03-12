import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Select as _Select } from "bumbag"
import AlertDialog from "../dialog/Alert"
import { useVipLevelList } from "./client"
import Select from "../select/Select"
import { useTrans } from "../i18n"

export const VipLevelSelect = ({ placeholder, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "等级",
        en: "VIP Level"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let { data, isLoading, error } = useVipLevelList()

    let options =
        data?.items?.map(({ id, level }) => ({
            label: `VIP ${level}`,
            value: id
        })) ?? []

    return (
        <>
            <Select
                isLoading={isLoading}
                options={options}
                placeholder={placeholder}
                {...props}
            />
            {error && <AlertDialog {...error} />}
        </>
    )
}

export default VipLevelSelect
