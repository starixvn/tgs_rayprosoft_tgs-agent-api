import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import {
    Select as _Select,
    Box,
    Set,
    Button,
    Text,
    Switch,
    Input,
    Stack
} from "bumbag"
import { useTrans } from "../../i18n"
import { useFormContext } from "react-hook-form"

export const AllPlayers = "AllPlayers"
const AllPlayersTrans = {
    zh: "全部玩家",
    en: "All players"
}

export const DesignatedPlayers = "DesignatedPlayers"
const DesignatedPlayersTrans = {
    zh: "指定玩家",
    en: "Designated Players"
}

export const Option = {
    AllPlayers,
    DesignatedPlayers
}

export const PlayerTargetSelect = ({
    name = "target",
    selectRef,
    required,
    ...props
}) => {
    let {
        register,
        formState: { errors }
    } = useFormContext()

    let formProps = {}
    if (name && !selectRef) {
        let { ref, ...rest } = register(name, { required })
        formProps = rest
        selectRef = ref
    }

    let options = [
        {
            label: useTrans(AllPlayersTrans),
            value: AllPlayers
        },
        {
            label: useTrans(DesignatedPlayersTrans),
            value: DesignatedPlayers
        }
    ]

    let error = errors[name]
    let state = error && "danger"
    return (
        <_Select
            options={options}
            selectRef={selectRef}
            {...formProps}
            state={state}
            placeholder=""
            {...props}
        />
    )
}

export default PlayerTargetSelect
