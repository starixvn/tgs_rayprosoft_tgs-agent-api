import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
    Box,
    Set,
    FieldStack,
    Table,
    Button,
    Icon,
    Group,
    useModalState
} from "bumbag"
import { Trans, useTrans } from "../../i18n"
import { NormalLabel } from "../../form/Label"
import { useFormContext, useFieldArray, Controller } from "react-hook-form"
import Checkbox from "../../checkbox"
import Input from "../../input/Input"
import { isAllChecked, isAllPropChecked } from "../../checkbox/isAllChecked"
import get from "lodash-es/get"
import { NumberInput } from "../../input/NumberInput"
import { faMinus, faCog, faPlus } from "@fortawesome/free-solid-svg-icons"
import Disclosure, { ConfigIconDisclosure } from "../../dialog/Disclosure"
import { CloseText, ConfirmText } from "../../button/ActionButtons"
import Backdrop from "../../dialog/Backdrop"
import FormDialog from "../../form/FormDialog"

export const GameProviderCurrencyBetLimitFieldsBox = ({
    currencyFieldName = "currencies",
    betLimitFieldName = "betLimits",
    width = "100%",
    disabled,
    ...props
}) => {
    /*
    let { control, watch } = useFormContext()

    let currencies = watch(currencyFieldName)
    if (!currencies || currencies.length == 0) {
        return null
    }
    */
    return (
        <Body
            currencyFieldName={currencyFieldName}
            betLimitFieldName={betLimitFieldName}
            disabled={disabled}
            width={width}
        />
    )
}

const Body = ({
    currencyFieldName,
    betLimitFieldName,
    disabled,
    width = "100%"
}) => {
    let {
        control,
        watch,
        formState: { errors }
    } = useFormContext()

    let currencyFieldArray = useFieldArray({
        control,
        name: currencyFieldName,
        keyName: "_id"
    })

    let betLimitFieldArray = useFieldArray({
        control,
        name: betLimitFieldName,
        keyName: "_id"
    })

    let currencies = watch(currencyFieldName)
    let betLimits = watch(betLimitFieldName)
    let isAllCurrencyChecked = isAllPropChecked(currencies)

    let currencyBetLimitsPairs = currencyFieldArray?.fields?.map(
        (currencyField) => {
            let betLimitFieldIndexPairs = betLimitFieldArray?.fields
                ?.map((field, index) => [field, index])
                .filter(
                    ([{ currencyCode }]) => currencyField.code === currencyCode
                )
            return [currencyField, betLimitFieldIndexPairs]
        }
    )

    return useMemo(() => {
        return (
            <>
                <NormalLabel>
                    <Trans zh="币种限红" en="Currency Bet Limit" />
                </NormalLabel>
                <Set orientation="vertical" width={width}>
                    <Table width={width} hasDividers>
                        <Table.Head>
                            <Table.Row>
                                <Table.HeadCell>
                                    <Box></Box>
                                </Table.HeadCell>
                                <Table.HeadCell colSpan="5">
                                    <Box marginLeft="1rem">
                                        <Trans zh="币种" en="Currency" />
                                    </Box>
                                </Table.HeadCell>
                            </Table.Row>
                        </Table.Head>
                        <Table.Body>
                            {currencyBetLimitsPairs.map(
                                (
                                    [currencyField, betLimitFieldIndexPairs],
                                    index
                                ) => {
                                    return (
                                        <CurrencyBetLimitsRow
                                            key={currencyField._id}
                                            currencyField={currencyField}
                                            currencyFieldIndex={index}
                                            currencyFieldName={
                                                currencyFieldName
                                            }
                                            betLimitFieldName={
                                                betLimitFieldName
                                            }
                                            betLimitFieldArray={
                                                betLimitFieldArray
                                            }
                                            betLimitFieldIndexPairs={
                                                betLimitFieldIndexPairs
                                            }
                                            disabled={disabled}
                                        />
                                    )
                                }
                            )}
                        </Table.Body>
                    </Table>
                </Set>
            </>
        )
    }, [isAllCurrencyChecked, JSON.stringify(currencyBetLimitsPairs), errors])
}

const CurrencyBetLimitsRow = ({
    currencyField,
    currencyFieldIndex,
    currencyFieldName,
    betLimitFieldName,
    betLimitFieldArray,
    betLimitFieldIndexPairs = [],
    disabled
}) => {
    let { register, watch, setValue } = useFormContext()

    let currencyCheckedFieldName = toCurrencyCheckedFieldName(
        currencyFieldName,
        currencyFieldIndex
    )

    let isCurrencyChecked = watch(currencyCheckedFieldName)
    let betLimitHeaderVisibility = isCurrencyChecked ? "visible" : "hidden"

    let currencyFieldRow = useMemo(() => {
        let currencyCodeFieldName = `${currencyFieldName}.${currencyFieldIndex}.code`
        return (
            <>
                <Table.Row key={currencyField._id}>
                    <Table.HeadCell>
                        <Box>
                            <Input
                                type="hidden"
                                name={currencyCodeFieldName}
                                value={currencyField.code}
                            />
                            {disabled && (
                                <>
                                    <Checkbox
                                        display="none"
                                        name={currencyCheckedFieldName}
                                        checked={currencyField.checked}
                                    />
                                    <Checkbox
                                        disabled
                                        checked={currencyField.checked}
                                    />
                                </>
                            )}
                            {!disabled && (
                                <>
                                    <Checkbox
                                        name={currencyCheckedFieldName}
                                        defaultChecked={currencyField.checked}
                                    />
                                </>
                            )}
                        </Box>
                    </Table.HeadCell>
                    <Table.HeadCell>
                        <CurrencyCodeDisclosure
                            currencyFieldName={currencyFieldName}
                            currencyField={currencyField}
                            currencyFieldIndex={currencyFieldIndex}
                            disabled={disabled}
                        />
                    </Table.HeadCell>
                    <Table.HeadCell textAlign="center">
                        <Box visibility={betLimitHeaderVisibility}>
                            <Trans zh="最低投注" en="Min Bet" />
                        </Box>
                    </Table.HeadCell>
                    <Table.HeadCell textAlign="center">
                        <Box visibility={betLimitHeaderVisibility}>
                            <Trans zh="最高投注" en="Max Bet" />
                        </Box>
                    </Table.HeadCell>
                    <Table.HeadCell textAlign="center">
                        <Box visibility={betLimitHeaderVisibility}>
                            <Trans zh="单场最高投注" en="Max Bet Per Match" />
                        </Box>
                    </Table.HeadCell>
                    <Table.HeadCell textAlign="center">
                        <Box visibility={betLimitHeaderVisibility}>
                            <Trans
                                zh="External Setting"
                                en="External Setting"
                            />
                        </Box>
                    </Table.HeadCell>
                </Table.Row>
            </>
        )
    }, [isCurrencyChecked])

    return (
        <>
            {currencyFieldRow}
            {betLimitFieldIndexPairs?.map(([betLimitField, index]) => (
                <BetLimitRow
                    key={betLimitField._id}
                    currencyField={currencyField}
                    currencyFieldIndex={currencyFieldIndex}
                    isCurrencyChecked={isCurrencyChecked}
                    currencyFieldName={currencyFieldName}
                    betLimitFieldName={betLimitFieldName}
                    betLimitFieldArray={betLimitFieldArray}
                    betLimitField={betLimitField}
                    betLimitFieldIndex={index}
                    disabled={disabled}
                />
            ))}
            {isCurrencyChecked && !disabled && (
                <AddBetLimitRow
                    currencyField={currencyField}
                    betLimitFieldArray={betLimitFieldArray}
                />
            )}
        </>
    )
}

const AddBetLimitRow = ({ currencyField, betLimitFieldArray }) => {
    return (
        <Table.Row>
            <Table.HeadCell></Table.HeadCell>
            <Table.HeadCell>
                <Button
                    size="small"
                    variant="ghost"
                    onClick={(event) => {
                        betLimitFieldArray.append({
                            currencyCode: currencyField.code
                        })
                    }}
                >
                    <Icon icon={faPlus} type="font-awesome" />
                </Button>
            </Table.HeadCell>
            <Table.HeadCell>
                <Input visibility="hidden" />
            </Table.HeadCell>
            <Table.HeadCell>
                <Input visibility="hidden" />
            </Table.HeadCell>
            <Table.HeadCell>
                <Input visibility="hidden" />
            </Table.HeadCell>
            <Table.HeadCell>
                <Input visibility="hidden" />
            </Table.HeadCell>
        </Table.Row>
    )
}

const BetLimitRow = ({
    currencyField,
    currencyFieldIndex,
    currencyFieldName,
    betLimitFieldName,
    betLimitFieldArray,
    betLimitField,
    betLimitFieldIndex,
    disabled
}) => {
    let {
        register,
        watch,
        setValue,
        getValues,
        formState: { errors },
        clearErrors,
        trigger
    } = useFormContext()
    let currencyCheckedFieldName = toCurrencyCheckedFieldName(
        currencyFieldName,
        currencyFieldIndex
    )

    let isCurrencyChecked = watch(currencyCheckedFieldName)
    let visibility = isCurrencyChecked ? "visible" : "collapse"

    useEffect(() => {
        if (!isCurrencyChecked) {
            let { fields, remove } = betLimitFieldArray
            fields.map((betLimitField, index) => {
                if (betLimitField.currencyCode === currencyField.code) {
                    remove(index)
                }
            })
        }
    }, [isCurrencyChecked])

    return useMemo(() => {
        return (
            <Table.Row visibility={visibility}>
                <Table.Cell></Table.Cell>
                <Table.Cell>
                    <Button
                        size="small"
                        palette="danger"
                        variant="ghost"
                        onClick={(event) => {
                            betLimitFieldArray.remove(betLimitFieldIndex)
                        }}
                    >
                        <Icon icon={faMinus} type="font-awesome" />
                    </Button>
                    <Box alignX="center">
                        {betLimitField.id && (
                            <Input
                                type="hidden"
                                name={toBetLimitFieldName({
                                    betLimitFieldName,
                                    betLimitFieldIndex,
                                    name: "id"
                                })}
                                value={betLimitField.id}
                            />
                        )}
                        {/* <Input
                            type="hidden"
                            name={toBetLimitFieldName({
                                betLimitFieldName,
                                betLimitFieldIndex,
                                name: "gameProviderId"
                            })}
                            value={betLimitField.gameProviderId}
                        /> */}
                        <Input
                            type="hidden"
                            name={toBetLimitFieldName({
                                betLimitFieldName,
                                betLimitFieldIndex,
                                name: "currencyCode"
                            })}
                            value={betLimitField.currencyCode}
                        />
                    </Box>
                </Table.Cell>
                <Table.Cell textAlign="center">
                    <NumberInput
                        name={toBetLimitFieldName({
                            betLimitFieldName,
                            betLimitFieldIndex,
                            name: "minBet"
                        })}
                        defaultValue={betLimitField.minBet}
                        min="0"
                        required
                        disabled={disabled}
                    />
                </Table.Cell>
                <Table.Cell textAlign="center">
                    <NumberInput
                        name={toBetLimitFieldName({
                            betLimitFieldName,
                            betLimitFieldIndex,
                            name: "maxBet"
                        })}
                        defaultValue={betLimitField.maxBet}
                        min="0"
                        required
                        disabled={disabled}
                    />
                </Table.Cell>
                <Table.Cell textAlign="center">
                    <NumberInput
                        name={toBetLimitFieldName({
                            betLimitFieldName,
                            betLimitFieldIndex,
                            name: "maxBetPerMatch"
                        })}
                        defaultValue={betLimitField.maxBetPerMatch}
                        min="0"
                        disabled={disabled}
                    />
                </Table.Cell>
                <Table.Cell textAlign="center">
                    <Input
                        name={toBetLimitFieldName({
                            betLimitFieldName,
                            betLimitFieldIndex,
                            name: "externalSetting"
                        })}
                        defaultValue={betLimitField.externalSetting}
                        disabled={disabled}
                    />
                </Table.Cell>
            </Table.Row>
        )
    }, [isCurrencyChecked])
}

const CurrencyCodeDisclosure = ({
    currencyFieldName,
    currencyField,
    currencyFieldIndex,
    disabled
}) => {
    let { control, setValue } = useFormContext()
    let name = `${currencyFieldName}.${currencyFieldIndex}.gameProviderCurrencyCode`

    let dialog = useModalState()
    return (
        <Controller
            control={control}
            name={name}
            shouldUnregister={true}
            defaultValue={currencyField.gameProviderCurrencyCode}
            render={({ field: { value }, fieldState, formState }) => (
                <>
                    <Group>
                        <Button size="small" isStatic>
                            {currencyField.code}
                        </Button>
                        <ConfigIconDisclosure size="small" {...dialog} />
                    </Group>
                    {dialog.visible && (
                        <CurrencyCodeDialog
                            value={value}
                            disabled={disabled}
                            onChange={(code) => {
                                if (!disabled) {
                                    setValue(name, code)
                                }
                            }}
                            dialog={dialog}
                        />
                    )}
                </>
            )}
        />
    )
}

const CurrencyCodeDialog = ({ value, disabled, onChange, dialog }) => {
    let title = useTrans({
        zh: `游戏商币种代码`,
        en: `Game Provider Currency Code`
    })

    return (
        <Backdrop {...dialog}>
            <FormDialog
                stopPropagation={true}
                onSubmit={({ code }) => {
                    onChange(code)
                    dialog.hide()
                }}
                submitText={<ConfirmText />}
                showActionButtons={!disabled}
                dialog={dialog}
                title={title}
                hideBackdrop={true}
                minWidth="max-content"
            >
                <Input name="code" defaultValue={value} disabled={disabled} />
            </FormDialog>
        </Backdrop>
    )
}

const toCurrencyCheckedFieldName = (name, index) => {
    return `${name}.${index}.checked`
}

const toBetLimitFieldName = ({
    betLimitFieldName,
    betLimitFieldIndex,
    name
}) => {
    return `${betLimitFieldName}.${betLimitFieldIndex}.${name}`
}
