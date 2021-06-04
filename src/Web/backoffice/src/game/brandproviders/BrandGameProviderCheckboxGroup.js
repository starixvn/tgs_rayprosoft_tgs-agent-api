import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
    Table,
    Set,
    Heading,
    Text,
    useModalState,
    Icon,
    FieldStack,
    Box,
    Group,
    Button,
    Spinner
} from "bumbag"
import { useBrandGameProviderList } from "./client"
import { Trans, useTrans } from "../../i18n"
import Checkbox from "../../checkbox"
import { useFormContext } from "react-hook-form"
import Input from "../../input/Input"
import { isAllPropChecked } from "../../checkbox/isAllChecked"

export const BrandGameProviderCheckboxGroupTable = ({
    brandId,
    name = "gameProviders",
    disabled,
    ...props
}) => {
    let { isLoading, data, error } = useBrandGameProviderList({
        brandId,
        showAll: true,
        sortby: "gameProviderName"
    })

    if (isLoading) {
        return <Spinner />
    }

    return <Body name={name} items={data?.items} disabled={disabled} />
}

const Body = ({ name, items = [], disabled, ...props }) => {
    let { control, setValue, watch, register } = useFormContext()

    let selection = watch(name)
    let allChecked = isAllPropChecked(selection)

    return (
        <Table>
            <Table.Head>
                <Table.Row>
                    <Table.HeadCell textAlign="center">
                        <Box>
                            <Checkbox
                                checked={allChecked}
                                onChange={(event) => {
                                    let checked = event.target.checked
                                    items.forEach((field, index) => {
                                        setValue(
                                            toCheckedFieldName(name, index),
                                            checked
                                        )
                                    })
                                }}
                                disabled={disabled}
                            />
                        </Box>
                    </Table.HeadCell>
                    <Table.HeadCell textAlign="center" width="100%">
                        <Trans zh="游戏厂商" en="Game Provider" />
                    </Table.HeadCell>
                </Table.Row>
            </Table.Head>
            <Table.Body>
                {items.map((item, index) => (
                    <Row
                        key={index}
                        name={name}
                        gameProvider={item}
                        index={index}
                        disabled={disabled}
                    />
                ))}
            </Table.Body>
        </Table>
    )
}

const Row = ({ name, gameProvider, index, disabled = false, ...props }) => {
    let gameProviderFieldName = `${name}.${index}`
    return (
        <Table.Row>
            <Table.Cell textAlign="center" width="fit-content">
                <Box>
                    <Checkbox
                        name={`${gameProviderFieldName}.checked`}
                        disabled={disabled}
                    />
                    <Input
                        type="hidden"
                        name={`${gameProviderFieldName}.gameProviderId`}
                        value={gameProvider.gameProviderId}
                    />
                </Box>
            </Table.Cell>
            <Table.Cell textAlign="center">
                {gameProvider.gameProviderName}
            </Table.Cell>
        </Table.Row>
    )
}

const toGameProviderFieldName = (name, index) => `${name}.${index}`

const toCheckedFieldName = (name, index) => {
    let gameProviderFieldName = toGameProviderFieldName(name, index)
    return `${gameProviderFieldName}.checked`
}
