import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import { Heading, Stack, Box, Button, Set, useModalState } from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import { usePlayerList } from "./client"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { PlayerStatusLabel } from "../PlayerStatus"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserBrandId
} from "../../user/context"
import { Code } from "./menu"
import { PlayerStatusSelect } from "../PlayerStatus"
import { VipLevelSelect } from "../../vip-levels/VipLevelSelect"
import { ViewButton, EditButton } from "./button"
import { Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { InputField } from "../../input/Input"
import BrandBar from "../../report/BrandBar"
import AlertDialog from "../../dialog/Alert"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = () => {
    let userBrandId = useUserBrandId()
    let [brandId, setBrandId] = useState(userBrandId)

    return (
        <Set orientation="vertical" isFilled>
            <BrandBar
                onBrandChange={(value) => {
                    setBrandId(value)
                }}
            />
            {brandId && <BodyContent brandId={brandId} />}
        </Set>
    )
}

const BodyContent = ({ brandId = "" }) => {
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        status: "",
        vipId: "",
        username: ""
    })

    useEffect(() => {
        setState({ pageIndex: 0, brandId })
    }, [brandId])

    let { pageIndex, pageSize, status, vipId, username = "" } = state
    let { data = {}, isLoading, mutate, error, deleteCache } = usePlayerList({
        ...toApiParams(state),
        sortby: "username",
        brandId
    })
    let columns = useColumns({ mutate, items: data?.items, brandId })

    return (
        <PageContent
            title={<Trans zh="玩家列表" en="Players" />}
            actionBar={
                <SearchBar
                    brandId={brandId}
                    search={(data) => {
                        let newState = {
                            pageIndex: 0,
                            status: data.status,
                            vipId: data.vipId,
                            username: data.username
                        }
                        if (
                            equals(newState, {
                                pageIndex,
                                status,
                                vipId,
                                username
                            })
                        ) {
                            mutate()
                        }
                        setState({
                            pageIndex: 0,
                            ...data
                        })
                    }}
                    isLoading={isLoading}
                />
            }
        >
            <Table
                controlledState={state}
                columns={columns}
                data={data?.items}
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

const SearchBar = ({ brandId, search, isLoading, ...props }) => {
    let usernameInputPlaceholder = useSearchInputPlaceHolder()

    let formMethods = useForm()

    const onSubmit = (data) => search(data)

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
                    <PlayerStatusSelect name="status" />
                    <VipLevelSelect name="vipId" brandId={brandId} />
                    <InputField
                        name="username"
                        showEmptyFieldValueAddonAfter
                        placeholder={usernameInputPlaceholder}
                        autocomplete="off"
                    />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useSearchInputPlaceHolder = () =>
    useTrans({ zh: "用户名输入框", en: "Input user name" })

const useColumns = ({ mutate, items = [], brandId } = {}) => {
    let hasAnyMenu = useHasSubMenuByRoute()

    return useMemo(() => {
        let columns = [
            sequenceColumn(),
            {
                Header: <Trans zh="用户名" en="User name" />,
                accessor: "username",
                disableSortBy: true,
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                }
            },
            {
                Header: <Trans zh="VIP 等级" en="VIP Level" />,
                disableSortBy: true,
                accessor: "vipLevel",
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                }
            },
            {
                disableSortBy: true,
                Header: <Trans zh="状态" en="Status" />,
                accessor: "status",
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                },
                Cell: ({ value }) => <PlayerStatusLabel value={value} />
            }
        ]

        if (hasAnyMenu) {
            columns.push({
                id: "actions",
                disableSortBy: true,
                Header: <Trans zh="操作" en="Actions" />,
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                },
                accessor: (row) => {
                    return row
                },
                Cell: ({ value }) => {
                    return (
                        <Box alignX="center">
                            <Set>
                                <ViewButton player={value} brandId={brandId} />
                                <EditButton
                                    brandId={brandId}
                                    player={value}
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
