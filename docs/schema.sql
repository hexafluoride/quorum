--
-- PostgreSQL database dump
--

-- Dumped from database version 10.3
-- Dumped by pg_dump version 10.3

-- Started on 2018-06-06 01:39:55

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 616 (class 1247 OID 16473)
-- Name: board_parent_type; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.board_parent_type AS ENUM (
    'group',
    'board'
);


ALTER TYPE public.board_parent_type OWNER TO postgres;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- TOC entry 203 (class 1259 OID 16477)
-- Name: board_groups; Type: TABLE; Schema: public; Owner: quorum_user
--

CREATE TABLE public.board_groups (
    id bigint NOT NULL,
    name character varying NOT NULL,
    description character varying,
    parent bigint
);


ALTER TABLE public.board_groups OWNER TO quorum_user;

--
-- TOC entry 202 (class 1259 OID 16464)
-- Name: boards; Type: TABLE; Schema: public; Owner: quorum_user
--

CREATE TABLE public.boards (
    id bigint NOT NULL,
    name character varying(100) NOT NULL,
    description character varying(1000) NOT NULL,
    parent_type public.board_parent_type NOT NULL,
    parent bigint NOT NULL,
    alias character varying
);


ALTER TABLE public.boards OWNER TO quorum_user;

--
-- TOC entry 196 (class 1259 OID 16396)
-- Name: logins; Type: TABLE; Schema: public; Owner: quorum_user
--

CREATE TABLE public.logins (
    username character varying(50) NOT NULL,
    password character varying(200) NOT NULL,
    email character varying(200),
    id bigint NOT NULL
);


ALTER TABLE public.logins OWNER TO quorum_user;

--
-- TOC entry 199 (class 1259 OID 16436)
-- Name: posts; Type: TABLE; Schema: public; Owner: quorum_user
--

CREATE TABLE public.posts (
    id bigint NOT NULL,
    author bigint NOT NULL,
    thread bigint NOT NULL,
    content character varying NOT NULL,
    content_type character varying NOT NULL,
    created date NOT NULL,
    board bigint NOT NULL,
    rendered_content character varying NOT NULL,
    title character varying NOT NULL
);


ALTER TABLE public.posts OWNER TO quorum_user;

--
-- TOC entry 197 (class 1259 OID 16401)
-- Name: sessions; Type: TABLE; Schema: public; Owner: quorum_user
--

CREATE TABLE public.sessions (
    id character varying(128) NOT NULL,
    created date NOT NULL,
    expires date,
    uid bigint NOT NULL
);


ALTER TABLE public.sessions OWNER TO quorum_user;

--
-- TOC entry 204 (class 1259 OID 16485)
-- Name: threads; Type: TABLE; Schema: public; Owner: quorum_user
--

CREATE TABLE public.threads (
    id bigint NOT NULL,
    board bigint NOT NULL,
    opening_post bigint
);


ALTER TABLE public.threads OWNER TO quorum_user;

--
-- TOC entry 200 (class 1259 OID 16446)
-- Name: user_map; Type: TABLE; Schema: public; Owner: quorum_user
--

CREATE TABLE public.user_map (
    method character varying NOT NULL,
    identifier character varying NOT NULL,
    uid bigint NOT NULL
);


ALTER TABLE public.user_map OWNER TO quorum_user;

--
-- TOC entry 201 (class 1259 OID 16452)
-- Name: users; Type: TABLE; Schema: public; Owner: quorum_user
--

CREATE TABLE public.users (
    uid bigint NOT NULL,
    username character varying(50) NOT NULL,
    bio character varying(500) NOT NULL
);


ALTER TABLE public.users OWNER TO quorum_user;

--
-- TOC entry 198 (class 1259 OID 16409)
-- Name: users_id_seq; Type: SEQUENCE; Schema: public; Owner: quorum_user
--

CREATE SEQUENCE public.users_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.users_id_seq OWNER TO quorum_user;

--
-- TOC entry 2856 (class 0 OID 0)
-- Dependencies: 198
-- Name: users_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: quorum_user
--

ALTER SEQUENCE public.users_id_seq OWNED BY public.logins.id;


--
-- TOC entry 2707 (class 2604 OID 16411)
-- Name: logins id; Type: DEFAULT; Schema: public; Owner: quorum_user
--

ALTER TABLE ONLY public.logins ALTER COLUMN id SET DEFAULT nextval('public.users_id_seq'::regclass);


--
-- TOC entry 2720 (class 2606 OID 16484)
-- Name: board_groups board_groups_pkey; Type: CONSTRAINT; Schema: public; Owner: quorum_user
--

ALTER TABLE ONLY public.board_groups
    ADD CONSTRAINT board_groups_pkey PRIMARY KEY (id);


--
-- TOC entry 2718 (class 2606 OID 16471)
-- Name: boards boards_pkey; Type: CONSTRAINT; Schema: public; Owner: quorum_user
--

ALTER TABLE ONLY public.boards
    ADD CONSTRAINT boards_pkey PRIMARY KEY (id);


--
-- TOC entry 2713 (class 2606 OID 16443)
-- Name: posts posts_pkey; Type: CONSTRAINT; Schema: public; Owner: quorum_user
--

ALTER TABLE ONLY public.posts
    ADD CONSTRAINT posts_pkey PRIMARY KEY (id);


--
-- TOC entry 2709 (class 2606 OID 16408)
-- Name: sessions sessions_pkey; Type: CONSTRAINT; Schema: public; Owner: quorum_user
--

ALTER TABLE ONLY public.sessions
    ADD CONSTRAINT sessions_pkey PRIMARY KEY (id);


--
-- TOC entry 2724 (class 2606 OID 16489)
-- Name: threads threads_pkey; Type: CONSTRAINT; Schema: public; Owner: quorum_user
--

ALTER TABLE ONLY public.threads
    ADD CONSTRAINT threads_pkey PRIMARY KEY (id);


--
-- TOC entry 2716 (class 2606 OID 16459)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: quorum_user
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (uid);


--
-- TOC entry 2710 (class 1259 OID 16445)
-- Name: board; Type: INDEX; Schema: public; Owner: quorum_user
--

CREATE INDEX board ON public.posts USING btree (board);


--
-- TOC entry 2711 (class 1259 OID 16512)
-- Name: fki_author_fkey_constraint; Type: INDEX; Schema: public; Owner: quorum_user
--

CREATE INDEX fki_author_fkey_constraint ON public.posts USING btree (author);


--
-- TOC entry 2721 (class 1259 OID 16501)
-- Name: fki_board_fkey_constraint; Type: INDEX; Schema: public; Owner: quorum_user
--

CREATE INDEX fki_board_fkey_constraint ON public.threads USING btree (board);


--
-- TOC entry 2722 (class 1259 OID 16495)
-- Name: fki_po; Type: INDEX; Schema: public; Owner: quorum_user
--

CREATE INDEX fki_po ON public.threads USING btree (opening_post);


--
-- TOC entry 2714 (class 1259 OID 16444)
-- Name: thread; Type: INDEX; Schema: public; Owner: quorum_user
--

CREATE INDEX thread ON public.posts USING btree (thread);


--
-- TOC entry 2726 (class 2606 OID 16507)
-- Name: posts author_fkey_constraint; Type: FK CONSTRAINT; Schema: public; Owner: quorum_user
--

ALTER TABLE ONLY public.posts
    ADD CONSTRAINT author_fkey_constraint FOREIGN KEY (author) REFERENCES public.users(uid);


--
-- TOC entry 2728 (class 2606 OID 16496)
-- Name: threads board_fkey_constraint; Type: FK CONSTRAINT; Schema: public; Owner: quorum_user
--

ALTER TABLE ONLY public.threads
    ADD CONSTRAINT board_fkey_constraint FOREIGN KEY (board) REFERENCES public.boards(id);


--
-- TOC entry 2725 (class 2606 OID 16502)
-- Name: posts board_fkey_constraint; Type: FK CONSTRAINT; Schema: public; Owner: quorum_user
--

ALTER TABLE ONLY public.posts
    ADD CONSTRAINT board_fkey_constraint FOREIGN KEY (board) REFERENCES public.boards(id);


--
-- TOC entry 2727 (class 2606 OID 16490)
-- Name: threads op_fkey_constraint; Type: FK CONSTRAINT; Schema: public; Owner: quorum_user
--

ALTER TABLE ONLY public.threads
    ADD CONSTRAINT op_fkey_constraint FOREIGN KEY (opening_post) REFERENCES public.posts(id);


-- Completed on 2018-06-06 01:39:56

--
-- PostgreSQL database dump complete
--

