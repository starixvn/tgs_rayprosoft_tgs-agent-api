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
    Card,
    Level
} from "bumbag"
import Disclosure, {
    AddDisclosure,
    EditDisclosure
} from "../../dialog/Disclosure"
import {
    useForm,
    useFormContext,
    useFieldArray,
    Controller,
    FormProvider
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
import { pageCount, Table, toApiParams, useTable } from "../../table"
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
import SearchButton from "../../button/SearchButton"

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

const GameListDialog = ({ gameProviderId, dialog, ...props }) => {
    let title = useTrans({
        zh: "游戏列表",
        en: "Game List"
    })

    let { state, setState, setPageIndex, setSortOrder } = useTable({})
    //TODO: use get game provider game list API
    let { isLoading, data, error, mutate, deleteCache } =
        useGameProvider(gameProviderId)
    // let { pageIndex, pageSize, name } = state
    // let { isLoading, data, error, mutate, deleteCache } = useGameProviderGames(
    //     gameProviderId,
    //     toApiParams(state)
    // )

    let items = data?.games ?? []
    let columns = useColumns({ items, mutate })
    if (isLoading) {
        return <LoadingBackdrop {...dialog} />
    }

    if (error) {
        return <AlertDialog {...error} onClickClose={dialog.hide} />
    }

    return (
        <Dialog.Modal
            title={title}
            {...dialog}
            showCloseButton
            hideOnClickOutside={false}
            placement="top"
            width="80vw"
            maxWidth="80vw"
        >
            <FieldStack>
                <ActionBar
                    gameProviderId={gameProviderId}
                    isLoading={isLoading}
                    search={(data) => {
                        setState({
                            pageIndex: 0,
                            ...data
                        })
                        mutate()
                    }}
                />
                <Table
                    columns={columns}
                    data={items}
                    pageCount={pageCount(state.pageSize, data?.totalCount)}
                    setPageIndex={setPageIndex}
                    setSortOrder={setSortOrder}
                    isLoading={isLoading}
                />
            </FieldStack>
        </Dialog.Modal>
    )
}

const ActionBar = ({ gameProviderId, search, mutate, isLoading }) => {
    let formMethods = useForm({
        defaultValues: {}
    })

    return (
        <Level>
            <FormProvider {...formMethods}>
                <form onSubmit={formMethods.handleSubmit(search)}>
                    {/* <Set>
                        <InputField
                            name="code"
                            showEmptyFieldValueAddonAfter
                            placeholder="Code"
                        />
                        <SearchButton
                            isLoading={isLoading}
                            disabled={isLoading}
                        />
                    </Set> */}
                </form>
            </FormProvider>
            <Set orientation="horizontal">
                <AddGameProviderGameDisclosure
                    gameProviderId={gameProviderId}
                    onSubmitted={mutate}
                />

                <BatchAddGameProviderGameListDisclosure
                    gameProviderId={gameProviderId}
                    onSubmitted={mutate}
                />
            </Set>
        </Level>
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
