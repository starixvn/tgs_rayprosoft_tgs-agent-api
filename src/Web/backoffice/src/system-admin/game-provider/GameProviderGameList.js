import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import {
    Stack,
    Heading,
    Box,
    Button,
    Text,
    Group,
    Set,
    Flex,
    FieldStack,
    useModalState,
    Dialog
} from "bumbag"
import Disclosure, {
    AddDisclosure,
    EditDisclosure
} from "../../dialog/Disclosure"
import {
    useForm,
    useFormContext,
    useFieldArray,
    Controller
} from "react-hook-form"
import FormDialog from "../../form/FormDialog"
import { Trans, useLocale, useTrans } from "../../i18n"
import Backdrop from "../../dialog/Backdrop"
import {
    useGameProvider,
    useUpdateGameProvider
} from "../../game/providers/client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import clone from "clone-deep"
import { Table } from "../../table"
import textColumn from "../../table/column/textColumn"
import { CloseText } from "../../button/ActionButtons"
import { CloseButtonFooter } from "../../form/ConfirmButtonFooter"
import { InputField, Input } from "../../input/Input"
import { CheckboxField } from "../../checkbox/Checkbox"
import { GameStatusSelectField } from "../../game/games/GameStatus"
import { GameTypeSelectField } from "../../game/GameType"
import {
    GamePlatformLabel,
    GamePlatformTypeSelectField
} from "../../game/games/GamePlatform"

export const GameProviderGameListDisclosure = ({ gameProviderId }) => {
    let dialog = useModalState()

    return (
        <>
            <Disclosure {...dialog}>
                <Trans zh="游戏列表" en="Game List" />
            </Disclosure>
            {dialog.visible && (
                <GameListDialog
                    gameProviderId={gameProviderId}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const GameListDialog = ({ brandId, gameProviderId, dialog, ...props }) => {
    let title = useTrans({
        zh: "游戏列表",
        en: "Game List"
    })

    let {
        isLoading: isLoadingGameProvider,
        data: gameProviderData,
        error: loadingGameProviderError
    } = useGameProvider(gameProviderId)

    let [formData, setSubmit] = useState()
    let { isLoading, result, error, mutate, deleteCache } =
        useUpdateGameProvider(gameProviderId, formData)

    if (isLoadingGameProvider) {
        return <LoadingBackdrop {...dialog} />
    }

    if (loadingGameProviderError) {
        return (
            <AlertDialog
                {...loadingGameProviderError}
                onClickClose={dialog.hide}
            />
        )
    }

    const onSubmit = (data) => {
        setSubmit(data)
    }

    const reset = () => {
        deleteCache()
        mutate(false, undefined)
        setSubmit(undefined)
    }

    return (
        <FormDialog
            defaultValues={clone(gameProviderData)}
            title={title}
            onSubmit={onSubmit}
            result={result}
            isLoading={isLoading}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={reset}
            dialog={dialog}
            placement="top"
            width="60vw"
            maxWidth="80vw"
        >
            <FormBody />
        </FormDialog>
    )
}

const GamesFieldName = "games"
const FormBody = () => {
    let { watch, setValue } = useFormContext()
    let games = watch(GamesFieldName)
    let columns = useColumns({ items: games })

    return (
        <FieldStack>
            <Box
                alignX="right"
                position="sticky"
                // top="0"
                // padding="1rem"
                // backgroundColor="#f7f7fa"
                // zIndex="1"
            >
                <AddGameDisclosure
                    onChange={({ id, ...data }) => {
                        let name = `${GamesFieldName}.${games.length}`
                        setValue(name, data)
                    }}
                />
            </Box>
            <Table columns={columns} data={games} />
        </FieldStack>
    )
}

const useColumns = ({ items = [], fieldArray = [] }) => {
    let { control, setValue } = useFormContext()
    return useMemo(() => {
        let columns = [
            textColumn({
                accessor: "id",
                Header: <Trans zh="游戏ID" en="ID" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "code",
                Header: <Trans zh="游戏代码" en="Code" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "name",
                Header: <Trans zh="游戏名称" en="Name" />,
                disableSortBy: true
            }),
            textColumn({
                id: "actions",
                accessor: (row) => row,
                Header: <Trans zh="操作" en="Actions" />,
                disableSortBy: true,
                Cell: ({ value: record, row }) => {
                    let index = row.index
                    let name = `${GamesFieldName}.${index}`
                    return (
                        <Set orientation="vertical" alignX="center">
                            <EditGameDisclosure
                                record={record}
                                onChange={(data) => {
                                    setValue(name, data)
                                }}
                            />
                        </Set>
                    )
                }
            })
        ]

        return columns
    }, [JSON.stringify(items)])
}

const AddGameDisclosure = ({ onChange }) => {
    let dialog = useModalState()

    return (
        <>
            <AddDisclosure {...dialog} />
            {dialog.visible && (
                <EditGameDialog isAdd onChange={onChange} dialog={dialog} />
            )}
        </>
    )
}

const EditGameDisclosure = ({ record, onChange }) => {
    let dialog = useModalState()

    return (
        <>
            <EditDisclosure {...dialog} />
            {dialog.visible && (
                <EditGameDialog
                    record={record}
                    onChange={onChange}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const EditGameDialog = ({
    title = "Game",
    record = {},
    onChange,
    isAdd,
    dialog
}) => {
    let defaultValue = clone(record)
    return (
        <Backdrop {...dialog}>
            <FormDialog
                defaultValues={defaultValue}
                title={title}
                onSubmit={(data) => {
                    onChange?.(data)
                    dialog.hide()
                }}
                stopPropagation
                hideBackdrop
                title={title}
                dialog={dialog}
                width="60vw"
                css={css`
                    & .bb-DialogContent {
                        position: relative;
                    }
                `}
            >
                <GameFieldBody isAdd={isAdd} />
            </FormDialog>
        </Backdrop>
    )
}

const GameFieldBody = ({ isAdd = false }) => {
    let { watch, getValues } = useFormContext()
    let isEdit = !isAdd
    return (
        <FieldStack>
            <FieldStack orientation="horizontal">
                {isEdit && (
                    <InputField
                        name="id"
                        value={watch("id")}
                        readOnly
                        label="Id"
                    />
                )}
                <InputField name="externalId" required label="External Id" />
            </FieldStack>
            <FieldStack orientation="horizontal">
                <InputField name="code" required label="Code" />
                <InputField name="name" required label="Name" />
            </FieldStack>
            <InputField name="description" label="Description" />
            <FieldStack orientation="horizontal">
                <GameStatusSelectField name="status" required label="Status" />
                <GameTypeSelectField name="type" required label="Type" />
                <GamePlatformTypeSelectField
                    name="platformType"
                    required
                    label="Platform Type"
                />
            </FieldStack>
            <InputField name="endpointPath" label="Endpoint Path" />
            <InputField name="ruleUrl" label="Rule Url" />
            <FieldStack orientation="horizontal">
                <CheckboxField name="subGame" label="Sub Game" />
                <CheckboxField
                    name="hasTurnoverOnTieBet"
                    label="Turnover On Tie Bet"
                />
                <CheckboxField name="html5" label="Html5" />
            </FieldStack>
            <InputField
                name="availableCurrencies"
                label="Available Currencies"
            />
        </FieldStack>
    )
}
