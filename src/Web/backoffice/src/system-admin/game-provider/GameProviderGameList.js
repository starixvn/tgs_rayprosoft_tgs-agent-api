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
    Dialog,
    Card
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
    useUpdateGameProvider,
    useUploadGameProviderGames
} from "../../game/providers/client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import clone from "clone-deep"
import { Table } from "../../table"
import textColumn from "../../table/column/textColumn"
import { InputField, Input } from "../../input/Input"
import { CheckboxField } from "../../checkbox/Checkbox"
import GameStatus, { GameStatusSelectField } from "../../game/games/GameStatus"
import { GameTypeSelectField } from "../../game/GameType"
import {
    GamePlatformLabel,
    GamePlatformTypeSelectField
} from "../../game/games/GamePlatform"
import { useDropzone } from "react-dropzone"
import {
    AddGameProviderGameDisclosure,
    GameProviderGameDisclosure
} from "./GameProviderGame"
import { BatchAddGameProviderGameListDisclosure } from "./BatchAddGameProviderGameList"

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
            width="80vw"
            maxWidth="80vw"
        >
            <FormBody gameProviderId={gameProviderId} />
        </FormDialog>
    )
}

const GamesFieldName = "games"
const FormBody = ({ gameProviderId }) => {
    let { watch, setValue } = useFormContext()
    let games = watch(GamesFieldName)
    let columns = useColumns({ items: games })

    return (
        <FieldStack>
            <Set
                alignX="right"
                position="sticky"
                orientation="horizontal"
                // top="0"
                // padding="1rem"
                // backgroundColor="#f7f7fa"
                // zIndex="1"
            >
                <AddGameProviderGameDisclosure
                    gameProviderId={gameProviderId}
                    onSubmitted={() => {
                        console.warn("TODO: mutate game list")
                    }}
                />

                <BatchAddGameProviderGameListDisclosure
                    gameProviderId={gameProviderId}
                    onSubmitted={() => {
                        console.warn("TODO: mutate game list")
                    }}
                />
            </Set>
            <Table columns={columns} data={games} />
        </FieldStack>
    )
}

const useColumns = ({ items = [], mutate }) => {
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
                Cell: ({ value: record }) => {
                    return (
                        <Set orientation="horizontal" alignX="center">
                            <GameProviderGameDisclosure
                                record={record}
                                onSubmitted={mutate}
                            />
                        </Set>
                    )
                }
            })
        ]

        return columns
    }, [JSON.stringify(items)])
}
