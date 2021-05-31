import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import { Box, Button, Set, Icon } from "bumbag"
import { faCheck } from "@fortawesome/free-solid-svg-icons"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import { AddButton, EditButton } from "./edit"
import { CopyButton } from "./copy"
import { DeleteButton } from "./delete"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserRoleType,
    useUserRoleId,
    useUserIsRootAdmin
} from "../../user/context"
import { Code, Update, Copy, Delete } from "./menu"
import { useRoleList } from "./client"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { useForm, useFormContext, FormProvider } from "react-hook-form"
import { InputField } from "../../input/Input"
import AlertDialog from "../../dialog/Alert"

export { Code }

export const Content = ({}) => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = () => {
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        name: ""
    })
    let { pageIndex, pageSize, name } = state
    let { isLoading, data, error, mutate, deleteCache } = useRoleList({
        ...toApiParams(state),
        sortby: "name"
    })
    let isRootAdmin = useUserIsRootAdmin()
    let userRoleId = useUserRoleId()
    let items = data?.items?.filter(({ id }) => {
        if (isRootAdmin) {
            //no filtering
            return true
        }
        return id != userRoleId
    })

    let columns = useColumns({ mutate, items })
    return (
        <PageContent
            title={<Trans zh="权限组管理" en="Roles" />}
            actionBar={
                <>
                    <SearchBar
                        search={(data = {}) => {
                            setState({
                                pageIndex: 0,
                                ...data
                            })
                            mutate()
                        }}
                        isPending={isLoading}
                    />
                    <AddButton onSubmitted={mutate} />
                </>
            }
        >
            <Table
                controlledState={state}
                columns={columns}
                data={items}
                pageCount={pageCount(pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
            />
            {error && (
                <AlertDialog
                    {...error}
                    onClickClose={() => {
                        deleteCache()
                        mutate({ data: undefined }, false)
                    }}
                />
            )}
        </PageContent>
    )
}

const SearchBar = ({ search, isLoading, ...props }) => {
    let formMethods = useForm({
        defaultValues: { name: "" }
    })

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(search)}>
                <Set>
                    <InputField
                        name="name"
                        showEmptyFieldValueAddonAfter
                        placeholder={useSearchInputPlaceHolder()}
                    />
                    <SearchButton isLoading={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useSearchInputPlaceHolder = () =>
    useTrans({
        zh: "权限组名称输入框",
        en: "Input role name"
    })

const useColumns = ({ mutate, items = [] }) => {
    let hasUpdateAuth = useIsDisplayMenuByRoute([Update])
    let hasCopyAuth = useIsDisplayMenuByRoute([Copy])
    let hasDeleteAuth = useIsDisplayMenuByRoute([Delete])
    let hasAnyMenu = hasUpdateAuth || hasCopyAuth || hasDeleteAuth

    return useMemo(() => {
        let columns = [
            sequenceColumn(),
            {
                id: "name",
                accessor: "name",
                Header: <Trans zh="权限组名称" en="Name" />,
                disableSortBy: true,
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                },
                width: 400
            },
            {
                id: "isRoot",
                accessor: "isRoot",
                Header: <Trans zh="根管理权限" en="Root" />,
                disableSortBy: true,
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                },
                Cell: ({ value: isRoot }) => {
                    return (
                        <Box alignX="center">
                            {isRoot ? (
                                <Icon icon={faCheck} type="font-awesome" />
                            ) : (
                                ""
                            )}
                        </Box>
                    )
                }
            }
        ]

        if (hasAnyMenu) {
            columns.push({
                id: "actions",
                disableSortBy: true,
                accessor: (row) => row,
                Header: <Trans zh="操作" en="Actions" />,
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                },
                Cell: ({ value }) => {
                    let { id, name, isRoot } = value
                    return (
                        <Box alignX="center">
                            <Set>
                                <EditButton
                                    id={id}
                                    record={value}
                                    onSubmitted={mutate}
                                />
                                <CopyButton
                                    id={id}
                                    name={name}
                                    onSubmitted={mutate}
                                    disabled={isRoot}
                                />
                                <DeleteButton
                                    id={id}
                                    name={name}
                                    onSubmitted={mutate}
                                />
                            </Set>
                        </Box>
                    )
                }
            })
        }
        return columns
    }, [JSON.stringify(items)])
}
