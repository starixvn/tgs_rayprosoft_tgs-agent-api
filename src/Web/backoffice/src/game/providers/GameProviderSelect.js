import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import Select from "../../select/Select"
import { Locales, Trans, useTrans } from "../../i18n"
import { useGameProviderList } from "./client"
import AlertDialog from "../../dialog/Alert"

export const GameProviderSelect = ({
    placeholder,
    valueField = "id",
    required,
    ...props
}) => {
    let defaultPlaceholder = useTrans({
        zh: "游戏厂商",
        en: "Game Provider"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let { data, isLoading, error } = useGameProviderList({ showAll: true })
    let options =
        data?.items.map((item) => ({
            label: item.name,
            value: item[valueField]
        })) ?? []

    return (
        <>
            {/* {error && <AlertDialog {...error} />} */}
            <Select
                isLoading={isLoading}
                placeholder={placeholder}
                options={options}
                {...props}
            />
        </>
    )
}

export default GameProviderSelect
